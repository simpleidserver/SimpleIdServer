// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.UserInfo
{
    [Route(SIDOpenIdConstants.EndPoints.UserInfo)]
    public class UserInfoController : Controller
    {
        private readonly IJwtParser _jwtParser;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly IEnumerable<IClaimsSource> _claimsSources;
        private readonly ITokenRepository _tokenRepository;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;
        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(
            IJwtParser jwtParser,
            IJwtBuilder jwtBuilder,
            IOAuthScopeRepository oauthScopeRepository,
            IOAuthUserRepository oauthUserRepository,
            IOAuthClientRepository oauthClientRepository,
            IEnumerable<IClaimsSource> claimsSources,
            ITokenRepository tokenRepository,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            ILogger<UserInfoController> logger)
        {
            _jwtParser = jwtParser;
            _jwtBuilder = jwtBuilder;
            _oauthScopeRepository = oauthScopeRepository;
            _oauthUserRepository = oauthUserRepository;
            _oauthClientRepository = oauthClientRepository;
            _claimsSources = claimsSources;
            _tokenRepository = tokenRepository;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
            _logger = logger;
        }

        [HttpGet]
        public Task<IActionResult> GetIndex(CancellationToken token)
        {
            return Common(null, token);
        }

        [HttpPost]
        public async Task<IActionResult> PostIndex(CancellationToken token)
        {
            try
            {
                var jObjBody = Request.Form?.ToJObject();
                return await Common(jObjBody, token);
            }
            catch(InvalidOperationException)
            {
                var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ErrorCodes.INVALID_REQUEST },
                    { ErrorResponseParameters.ErrorDescription, ErrorMessages.CONTENT_TYPE_NOT_SUPPORTED }
                };
                return new ContentResult
                {
                    Content = jObj.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }

        private async Task<IActionResult> Common(JObject content, CancellationToken cancellationToken)
        {
            try
            {
                var accessToken = ExtractAccessToken(content);
                var jwsPayload = await Extract(accessToken, cancellationToken);
                if (jwsPayload == null)
                {
                    throw new OAuthException(ErrorCodes.INVALID_TOKEN, OAuth.ErrorMessages.BAD_TOKEN);
                }

                var subject = jwsPayload.GetSub();
                var scopes = jwsPayload.GetScopes();
                var audiences = jwsPayload.GetAudiences();
                var claims = jwsPayload.GetClaimsFromAccessToken(AuthorizationRequestClaimTypes.UserInfo);
                var authTime = jwsPayload.GetAuthTime();
                var user = await _oauthUserRepository.FindOAuthUserByLogin(subject, cancellationToken);
                if (user == null)
                {
                    return new UnauthorizedResult();
                }

                var filteredClients = await _oauthClientRepository.FindOAuthClientByIds(audiences, cancellationToken);
                if (!filteredClients.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.INVALID_AUDIENCE);
                }

                var oauthClient = (OpenIdClient)filteredClients.First();
                if (!user.HasOpenIDConsent(oauthClient.ClientId, scopes, claims, AuthorizationRequestClaimTypes.UserInfo))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.NO_CONSENT);
                }

                var token = await _tokenRepository.Get(accessToken, cancellationToken);
                if (token == null)
                {
                    _logger.LogError("Cannot get user information because access token has been rejected");
                    throw new OAuthException(ErrorCodes.INVALID_TOKEN, OAuth.ErrorMessages.ACCESS_TOKEN_REJECTED);
                }

                var oauthScopes = await _oauthScopeRepository.FindOAuthScopesByNames(scopes, cancellationToken);
                var payload = new JwsPayload();
                IdTokenBuilder.EnrichWithScopeParameter(payload, oauthScopes, user, subject);
                _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(payload, claims, user, authTime, AuthorizationRequestClaimTypes.UserInfo);
                foreach (var claimsSource in _claimsSources)
                {
                    await claimsSource.Enrich(payload, oauthClient, cancellationToken);
                }

                string contentType = "application/json";
                var result = JsonConvert.SerializeObject(payload).ToString();
                if (!string.IsNullOrWhiteSpace(oauthClient.UserInfoSignedResponseAlg))
                {
                    payload.Add(Jwt.Constants.OAuthClaims.Issuer, Request.GetAbsoluteUriWithVirtualPath());
                    payload.Add(Jwt.Constants.OAuthClaims.Audiences, new string[]
                    {
                        token.ClientId
                    });
                    result = await _jwtBuilder.BuildClientToken(oauthClient, payload, oauthClient.UserInfoSignedResponseAlg, oauthClient.UserInfoEncryptedResponseAlg, oauthClient.UserInfoEncryptedResponseEnc, cancellationToken);
                    contentType = "application/jwt";
                }

                return new ContentResult
                {
                    Content = result,
                    ContentType = contentType
                };
            }
            catch (OAuthException ex)
            {
                var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new ContentResult
                {
                    Content = jObj.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }

        private async Task<JwsPayload> Extract(string accessToken, CancellationToken cancellationToken)
        {
            var isJwe = _jwtParser.IsJweToken(accessToken);
            var isJws = _jwtParser.IsJwsToken(accessToken);
            if (!isJwe && !isJws)
            {
                return null;
            }

            var jws = accessToken;
            if (isJwe)
            {
                jws = await _jwtParser.Decrypt(accessToken, cancellationToken);
            }

            return await _jwtParser.Unsign(jws, cancellationToken);
        }

        private string ExtractAccessToken(JObject jObj)
        {
            if (Request.Headers.ContainsKey(Constants.AuthorizationHeaderName))
            {
                foreach (var authorizationValue in Request.Headers[Constants.AuthorizationHeaderName])
                {
                    var at = authorizationValue.ExtractAuthorizationValue(new string[] { AutenticationSchemes.Bearer });
                    if (!string.IsNullOrWhiteSpace(at))
                    {
                        return at;
                    }
                }
            }

            if (jObj != null && jObj.ContainsKey(OAuth.DTOs.TokenResponseParameters.AccessToken))
            {
                var at = jObj.GetValue(OAuth.DTOs.TokenResponseParameters.AccessToken) as JValue;
                if(at != null && !string.IsNullOrWhiteSpace(at.ToString()))
                {
                    return at.ToString();
                }
            }

            throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_TOKEN);
        }
    }
}