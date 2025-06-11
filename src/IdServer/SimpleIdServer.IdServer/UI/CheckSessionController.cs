// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.IdServer.UI
{
    public class CheckSessionController : BaseController
    {
        private readonly IdServerHostOptions _options;
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IRealmStore _realmStore;

        public CheckSessionController(
            IOptions<IdServerHostOptions> options,
            ILanguageRepository languageRepository,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IClientRepository clientRepository,
            IAuthenticationHelper authenticationHelper,
            IRecurringJobManager reccuringJobManager,
            ITransactionBuilder transactionBuilder,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IRealmStore realmStore) : base(tokenRepository, jwtBuilder)
        {
            _options = options.Value;
            _languageRepository = languageRepository;
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _clientRepository = clientRepository;
            _authenticationHelper = authenticationHelper;
            _recurringJobManager = reccuringJobManager;
            _transactionBuilder = transactionBuilder;
            _realmStore = realmStore;
        }

        [HttpGet]
        public IActionResult Index([FromRoute] string prefix)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
            var newHtml = Html.Replace("{cookieName}", _options.GetSessionCookieName(_realmStore.Realm, userId));
            var activeSessionUrl = $"{issuer}/{Config.DefaultEndpoints.ActiveSession}";
            if (!string.IsNullOrWhiteSpace(prefix)) activeSessionUrl = $"{issuer}/{prefix}/{Config.DefaultEndpoints.ActiveSession}";
            newHtml = newHtml.Replace("{activeSessionUrl}", activeSessionUrl);
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = newHtml
            };
        }

        [HttpGet]
        public async Task<IActionResult> IsActive([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            if (!User.Identity.IsAuthenticated) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, Global.UserNotAuthenticated);
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var kvp = Request.Cookies.SingleOrDefault(c => c.Key == _options.GetSessionCookieName(_realmStore.Realm, userId));
            if (string.IsNullOrWhiteSpace(kvp.Value)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.MissingSessionId);
            var user = await _userRepository.GetBySubject(userId, prefix, cancellationToken);
            if (user == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.UNKNOWN_USER, Global.UserNotAuthenticated);
            var session = await _userSessionRepository.GetById(kvp.Value, prefix, cancellationToken);
            if (session == null || !session.IsActive()) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INACTIVE_SESSION, Global.InactiveSession);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> EndSession([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var url = $"/{Config.DefaultEndpoints.EndSessionCallback}";
            var jObjBody = Request.Query.ToJObject();
            var idTokenHint = jObjBody.GetIdTokenHintFromRpInitiatedLogoutRequest();
            var postLogoutRedirectUri = jObjBody.GetPostLogoutRedirectUriFromRpInitiatedLogoutRequest();
            var state = jObjBody.GetStateFromRpInitiatedLogoutRequest();
            try
            {
                var subject = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(postLogoutRedirectUri))
                {
                    Response.Cookies.Delete(_options.GetSessionCookieName(_realmStore.Realm, subject));
                    await HttpContext.SignOutAsync();
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = "You are logged out"
                    };
                }

                if (!User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        postLogoutRedirectUri = $"{postLogoutRedirectUri}?{RPInitiatedLogoutRequest.State}={HttpUtility.UrlEncode(state)}";
                    }

                    return Redirect(postLogoutRedirectUri);
                }

                var validationResult = await Validate(prefix, postLogoutRedirectUri, idTokenHint, cancellationToken);
                if (Request.QueryString.HasValue)
                {
                    url = $"/{Config.DefaultEndpoints.EndSessionCallback}?{string.Join("&", Request.Query.Select(q => $"{q.Key}={q.Value}"))}";
                }

                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    url = $"/{prefix}{url}";
                }

                var kvp = Request.Cookies.SingleOrDefault(c => c.Key == _options.GetSessionCookieName(_realmStore.Realm, subject));
                var userSession = await _userSessionRepository.GetById(kvp.Value, prefix, cancellationToken);
                IEnumerable<string> frontChannelLogouts = new List<string>();
                if (userSession != null && userSession.State == UserSessionStates.Active)
                {
                    var clientIds = userSession.ClientIds;
                    var targetedClients = await _clientRepository.GetByClientIdsAndExistingFrontchannelLogoutUri(prefix, clientIds, cancellationToken);
                    frontChannelLogouts = targetedClients.Select(c => BuildFrontChannelLogoutUrl(c, kvp.Value));
                    if (frontChannelLogouts.Any())
                    {
                        Response.SetNoCache();
                    }
                }

                var authenticatedUser = await _authenticationHelper.GetUserByLogin(subject, prefix, cancellationToken);
                if(!frontChannelLogouts.Any() && !validationResult.Client.RedirectToRevokeSessionUI)
                {
                    var issuer = Request.GetAbsoluteUriWithVirtualPath();
                    return Redirect($"{issuer}{url}");
                }

                var languages = await _languageRepository.GetAll(cancellationToken);
                return View(new RevokeSessionViewModel(
                    url,
                    validationResult.Payload,
                    frontChannelLogouts,
                    validationResult.Client.RedirectToRevokeSessionUI,
                    languages));
            }
            catch (OAuthException ex)
            {
                return BuildError(ex.Code, ex.Message);
            }
        }

        [Authorize(Constants.AuthenticatedPolicyName)]
        [HttpGet]
        public async Task<IActionResult> EndSessionCallback([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var jObjBody = Request.Query.ToJObject();
            var idTokenHint = jObjBody.GetIdTokenHintFromRpInitiatedLogoutRequest();
            var postLogoutRedirectUri = jObjBody.GetPostLogoutRedirectUriFromRpInitiatedLogoutRequest();
            var state = jObjBody.GetStateFromRpInitiatedLogoutRequest();
            try
            {
                var validationResult = await Validate(prefix, postLogoutRedirectUri, idTokenHint, cancellationToken);
                var subject = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var kvp = Request.Cookies.SingleOrDefault(c => c.Key == _options.GetSessionCookieName(_realmStore.Realm, subject));
                if(string.IsNullOrWhiteSpace(kvp.Value))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.NoSessionId);
                }

                using (var transaction = _transactionBuilder.Build())
                {
                    var activeSession = await _userSessionRepository.GetById(kvp.Value, prefix, cancellationToken);
                    if (activeSession == null)
                    {
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserSession, kvp.Value));
                    }

                    if (!activeSession.IsClientsNotified)
                    {
                        activeSession.State = UserSessionStates.Rejected;
                        _userSessionRepository.Update(activeSession);
                        await transaction.Commit(cancellationToken);
                        _recurringJobManager.Trigger(nameof(UserSessionJob));
                    }
                }

                Response.Cookies.Delete(_options.GetSessionCookieName(_realmStore.Realm, subject));
                await HttpContext.SignOutAsync();
                if (!string.IsNullOrWhiteSpace(state))
                {
                    postLogoutRedirectUri = $"{postLogoutRedirectUri}?{RPInitiatedLogoutRequest.State}={HttpUtility.UrlEncode(state)}";
                }

                return Redirect(postLogoutRedirectUri);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex.Code, ex.Message);
            }
        }

        protected string BuildFrontChannelLogoutUrl(Client client, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(client.FrontChannelLogoutUri))
                return null;

            var url = client.FrontChannelLogoutUri;
            if (client.FrontChannelLogoutSessionRequired)
            {
                var issuer = HandlerContext.GetIssuer(_realmStore.Realm, Request.GetAbsoluteUriWithVirtualPath(), _options.RealmEnabled);
                url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
                {
                    { JwtRegisteredClaimNames.Iss, issuer },
                    { JwtRegisteredClaimNames.Sid, sessionId }
                });
            }

            return url;
        }

        protected virtual async Task<ValidationResult> Validate(string realm, string postLogoutRedirectUri, string idTokenHint, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(postLogoutRedirectUri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.MissingPostLogoutRedirectUri);

            if (string.IsNullOrWhiteSpace(idTokenHint))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.MissingIdTokenHint);

            var extractionResult = ExtractIdTokenHint(realm, idTokenHint);
            var claimName = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (claimName != extractionResult.Jwt.Subject)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidSubjectIdTokenHint);

            if (!extractionResult.Jwt.Audiences.Contains(GetIssuer()))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidAudienceIdTokenHint);

            var clients = await _clientRepository.GetByClientIds(realm, extractionResult.Jwt.Audiences.ToList(), cancellationToken);
            if (clients == null || !clients.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidClientIdTokenHint);

            var openidClient = clients.FirstOrDefault(c => c.PostLogoutRedirectUris.Contains(postLogoutRedirectUri));
            if (openidClient == null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidPostLogoutRedirectUrl);

            if (extractionResult.EncryptedJwt != null)
            {
                if (openidClient.IdTokenEncryptedResponseAlg != extractionResult.EncryptedJwt.Alg || openidClient.IdTokenEncryptedResponseEnc != extractionResult.EncryptedJwt.Enc)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidEncOrAlgUsedToEncryptIdTokenHint);
            }

            if ((openidClient.IdTokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg) != extractionResult.Jwt.Alg)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidAlgUsedToSignIdTokenHint);
            }

            return new ValidationResult(extractionResult.Jwt, openidClient);

            string GetIssuer()
            {
                var request = Request.GetAbsoluteUriWithVirtualPath();
                return HandlerContext.GetIssuer(_realmStore.Realm, request, _options.RealmEnabled);
            }
        }

        private ReadJsonWebTokenResult ExtractIdTokenHint(string realm, string idTokenHint)
        {
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(idTokenHint))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidIdTokenHint);

            var validationResult = JwtBuilder.ReadSelfIssuedJsonWebToken(realm, idTokenHint);
            if(validationResult.Error != null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidIdTokenHint);
            return validationResult;
        }

        private static IActionResult BuildError(string code, string message)
        {
            var jObj = new JsonObject
            {
                [ErrorResponseParameters.Error] = code,
                [ErrorResponseParameters.ErrorDescription] = message
            };
            return new BadRequestObjectResult(jObj);
        }

        protected class ValidationResult
        {
            public ValidationResult(JsonWebToken payload, Client client)
            {
                Payload = payload;
                Client = client;
            }

            public JsonWebToken Payload { get; set; }
            public Client Client { get; set; }
        }

        protected class CheckSessionResult
        {
            public bool IsValid { get; set; }
        }

        private const string Html = @"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <title>Check Session IFrame</title>
</head>
<body>
    <script id='cookie-name' type='application/json'>{cookieName}</script>
    <script id='activesession-url' type='application/json'>{activeSessionUrl}</script>
    <script>
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/*  SHA-256 implementation in JavaScript                (c) Chris Veness 2002-2014 / MIT Licence  */
/*                                                                                                */
/*  - see http://csrc.nist.gov/groups/ST/toolkit/secure_hashing.html                              */
/*        http://csrc.nist.gov/groups/ST/toolkit/examples.html                                    */
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/* jshint node:true *//* global define, escape, unescape */
'use strict';
/**
 * SHA-256 hash function reference implementation.
 *
 * @namespace
 */
var Sha256 = {};
/**
 * Generates SHA-256 hash of string.
 *
 * @param   {string} msg - String to be hashed
 * @returns {string} Hash of msg as hex character string
 */
Sha256.hash = function(msg) {
    // convert string to UTF-8, as SHA only deals with byte-streams
    msg = msg.utf8Encode();
    
    // constants [§4.2.2]
    var K = [
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2 ];
    // initial hash value [§5.3.1]
    var H = [
        0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19 ];
    // PREPROCESSING 
 
    msg += String.fromCharCode(0x80);  // add trailing '1' bit (+ 0's padding) to string [§5.1.1]
    // convert string msg into 512-bit/16-integer blocks arrays of ints [§5.2.1]
    var l = msg.length/4 + 2; // length (in 32-bit integers) of msg + ‘1’ + appended length
    var N = Math.ceil(l/16);  // number of 16-integer-blocks required to hold 'l' ints
    var M = new Array(N);
    for (var i=0; i<N; i++) {
        M[i] = new Array(16);
        for (var j=0; j<16; j++) {  // encode 4 chars per integer, big-endian encoding
            M[i][j] = (msg.charCodeAt(i*64+j*4)<<24) | (msg.charCodeAt(i*64+j*4+1)<<16) | 
                      (msg.charCodeAt(i*64+j*4+2)<<8) | (msg.charCodeAt(i*64+j*4+3));
        } // note running off the end of msg is ok 'cos bitwise ops on NaN return 0
    }
    // add length (in bits) into final pair of 32-bit integers (big-endian) [§5.1.1]
    // note: most significant word would be (len-1)*8 >>> 32, but since JS converts
    // bitwise-op args to 32 bits, we need to simulate this by arithmetic operators
    M[N-1][14] = ((msg.length-1)*8) / Math.pow(2, 32); M[N-1][14] = Math.floor(M[N-1][14]);
    M[N-1][15] = ((msg.length-1)*8) & 0xffffffff;
    // HASH COMPUTATION [§6.1.2]
    var W = new Array(64); var a, b, c, d, e, f, g, h;
    for (var i=0; i<N; i++) {
        // 1 - prepare message schedule 'W'
        for (var t=0;  t<16; t++) W[t] = M[i][t];
        for (var t=16; t<64; t++) W[t] = (Sha256.σ1(W[t-2]) + W[t-7] + Sha256.σ0(W[t-15]) + W[t-16]) & 0xffffffff;
        // 2 - initialise working variables a, b, c, d, e, f, g, h with previous hash value
        a = H[0]; b = H[1]; c = H[2]; d = H[3]; e = H[4]; f = H[5]; g = H[6]; h = H[7];
        // 3 - main loop (note 'addition modulo 2^32')
        for (var t=0; t<64; t++) {
            var T1 = h + Sha256.sum1(e) + Sha256.Ch(e, f, g) + K[t] + W[t];
            var T2 =     Sha256.sum0(a) + Sha256.Maj(a, b, c);
            h = g;
            g = f;
            f = e;
            e = (d + T1) & 0xffffffff;
            d = c;
            c = b;
            b = a;
            a = (T1 + T2) & 0xffffffff;
        }
         // 4 - compute the new intermediate hash value (note 'addition modulo 2^32')
        H[0] = (H[0]+a) & 0xffffffff;
        H[1] = (H[1]+b) & 0xffffffff; 
        H[2] = (H[2]+c) & 0xffffffff; 
        H[3] = (H[3]+d) & 0xffffffff; 
        H[4] = (H[4]+e) & 0xffffffff;
        H[5] = (H[5]+f) & 0xffffffff;
        H[6] = (H[6]+g) & 0xffffffff; 
        H[7] = (H[7]+h) & 0xffffffff; 
    }
    return Sha256.toHexStr(H[0]) + Sha256.toHexStr(H[1]) + Sha256.toHexStr(H[2]) + Sha256.toHexStr(H[3]) + 
           Sha256.toHexStr(H[4]) + Sha256.toHexStr(H[5]) + Sha256.toHexStr(H[6]) + Sha256.toHexStr(H[7]);
};
/**
 * Rotates right (circular right shift) value x by n positions [§3.2.4].
 * @private
 */
Sha256.ROTR = function(n, x) {
    return (x >>> n) | (x << (32-n));
};
/**
 * Logical functions [§4.1.2].
 * @private
 */
Sha256.sum0  = function(x) { return Sha256.ROTR(2,  x) ^ Sha256.ROTR(13, x) ^ Sha256.ROTR(22, x); };
Sha256.sum1  = function(x) { return Sha256.ROTR(6,  x) ^ Sha256.ROTR(11, x) ^ Sha256.ROTR(25, x); };
Sha256.σ0  = function(x) { return Sha256.ROTR(7,  x) ^ Sha256.ROTR(18, x) ^ (x>>>3);  };
Sha256.σ1  = function(x) { return Sha256.ROTR(17, x) ^ Sha256.ROTR(19, x) ^ (x>>>10); };
Sha256.Ch  = function(x, y, z) { return (x & y) ^ (~x & z); };
Sha256.Maj = function(x, y, z) { return (x & y) ^ (x & z) ^ (y & z); };
/**
 * Hexadecimal representation of a number.
 * @private
 */
Sha256.toHexStr = function(n) {
    // note can't use toString(16) as it is implementation-dependant,
    // and in IE returns signed numbers when used on full words
    var s='', v;
    for (var i=7; i>=0; i--) { v = (n>>>(i*4)) & 0xf; s += v.toString(16); }
    return s;
};
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/** Extend String object with method to encode multi-byte string to utf8
 *  - monsur.hossa.in/2012/07/20/utf-8-in-javascript.html */
if (typeof String.prototype.utf8Encode == 'undefined') {
    String.prototype.utf8Encode = function() {
        return unescape( encodeURIComponent( this ) );
    };
}
/** Extend String object with method to decode utf8 string to multi-byte */
if (typeof String.prototype.utf8Decode == 'undefined') {
    String.prototype.utf8Decode = function() {
        try {
            return decodeURIComponent( escape( this ) );
        } catch (e) {
            return this; // invalid UTF-8? return as-is
        }
    };
}
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
if (typeof module != 'undefined' && module.exports) module.exports = Sha256; // CommonJs export
if (typeof define == 'function' && define.amd) define([], function() { return Sha256; }); // AMD
////////////////////////////////////////////////////////////////////
///////////// IdentityServer JS Code Starts here ///////////////////
////////////////////////////////////////////////////////////////////
        function getCookies() {
            var allCookies = document.cookie;
            var cookies = allCookies.split(';');
            return cookies.map(function(value) {
                var parts = value.trim().split('=');
                if (parts.length === 2) {
                    return {
                        name: parts[0].trim(),
                        value: parts[1].trim()
                    };
                }
            }).filter(function(item) {
                return item && item.name && item.value;
            });
        }
        function getBrowserSessionId() {
            var cookies = getCookies().filter(function(cookie) {
                return (cookie.name === cookieName);
            });
            return cookies[0] && cookies[0].value;
        }
        /*! (c) Tom Wu | http://www-cs-students.stanford.edu/~tjw/jsbn/ */
        var b64map = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
        var b64pad = '=';
        function hex2b64(h) {
            var i;
            var c;
            var ret = '';
            for (i = 0; i + 3 <= h.length; i += 3) {
                c = parseInt(h.substring(i, i + 3), 16);
                ret += b64map.charAt(c >> 6) + b64map.charAt(c & 63);
            }
            if (i + 1 == h.length) {
                c = parseInt(h.substring(i, i + 1), 16);
                ret += b64map.charAt(c << 2);
            }
            else if (i + 2 == h.length) {
                c = parseInt(h.substring(i, i + 2), 16);
                ret += b64map.charAt(c >> 2) + b64map.charAt((c & 3) << 4);
            }
            if (b64pad) while ((ret.length & 3) > 0) ret += b64pad;
            return ret;
        }
        function base64UrlEncode(s){
            var val = hex2b64(s);
            val = val.replace(/=/g, ''); // Remove any trailing '='s
            val = val.replace(/\+/g, '-'); // '+' => '-'
            val = val.replace(/\//g, '_'); // '/' => '_'
            return val;
        }
        function hash(value) {
            var hash = Sha256.hash(value);
            return base64UrlEncode(hash);
        }
        function computeSessionStateHash(clientId, origin, sessionId, salt) {
            return hash(clientId + origin + sessionId + salt);
        }

        async function calculateSessionStateResult(origin, message) {
            try {
                if (!origin || !message) {
                    return 'error';
                }
                var idx = message.lastIndexOf(' ');
                if (idx < 0 || idx >= message.length) {
                    return 'error';
                }
                var clientId = message.substring(0, idx);
                var sessionState = message.substring(idx + 1);
                if (!clientId || !sessionState) {
                    return 'error';
                }
                var sessionStateParts = sessionState.split('.');
                if (sessionStateParts.length !== 2) {
                    return 'error';
                }
                var clientHash = sessionStateParts[0];
                var salt = sessionStateParts[1];
                if (!clientHash || !salt) {
                    return 'error';
                }

                var response = await fetch(activeSessionUrl);
                if(!response.ok) {
                    return 'changed';
                }

                var currentSessionId = getBrowserSessionId();
                var expectedHash = computeSessionStateHash(clientId, origin, currentSessionId, salt);
                return clientHash === expectedHash ? 'unchanged' : 'changed';
            }
            catch (e) {
                return 'error';
            }
        }

        var cookieNameElem = document.getElementById('cookie-name');
        var activeSessionUrl = document.getElementById('activesession-url').textContent.trim();
        if (cookieNameElem) {
            var cookieName = cookieNameElem.textContent.trim();
        }

        if (cookieName && window.parent !== window) {
            window.addEventListener('message', async function (e) {
                var result = await calculateSessionStateResult(e.origin, e.data);
                e.source.postMessage(result, e.origin);
            }, false);
        }
    </script>
</body>
</html>
";
    }
}
