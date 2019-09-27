// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.UserInfo
{
    [Route(SIDOpenIdConstants.EndPoints.UserInfo)]
    public class UserInfoController : Controller
    {
        private readonly IJwtParser _jwtParser;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IOAuthScopeQueryRepository _oauthScopeRepository;
        private readonly IOAuthUserQueryRepository _oauthUserRepository;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IEnumerable<IClaimsSource> _claimsSources;

        public UserInfoController(IJwtParser jwtParser, IJwtBuilder jwtBuilder, IOAuthScopeQueryRepository oauthScopeRepository, IOAuthUserQueryRepository oauthUserRepository, IOAuthClientQueryRepository oauthClientRepository, IEnumerable<IClaimsSource> claimsSources)
        {
            _jwtParser = jwtParser;
            _jwtBuilder = jwtBuilder;
            _oauthScopeRepository = oauthScopeRepository;
            _oauthUserRepository = oauthUserRepository;
            _oauthClientRepository = oauthClientRepository;
            _claimsSources = claimsSources;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            try
            {
                var accessToken = ExtractAccessToken();
                var jwsPayload = await Extract(accessToken);
                if (jwsPayload == null)
                {
                    throw new OAuthException(ErrorCodes.INVALID_TOKEN, OAuth.ErrorMessages.BAD_TOKEN);
                }

                var subject = jwsPayload.GetSub();
                var scopes = jwsPayload.GetScopes();
                var audiences = jwsPayload.GetAudiences();
                var claims = jwsPayload.GetClaims();
                var authTime = jwsPayload.GetAuthTime();
                var user = await _oauthUserRepository.FindOAuthUserByLogin(subject);
                if (user == null)
                {
                    return new UnauthorizedResult();
                }

                var filteredClients = await _oauthClientRepository.FindOAuthClientByIds(audiences);
                if (!filteredClients.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.INVALID_AUDIENCE);
                }

                var oauthClient = (OpenIdClient)filteredClients.First();
                if (!user.HasOpenIDConsent(oauthClient.ClientId, scopes, claims, AuthorizationRequestClaimTypes.UserInfo))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.NO_CONSENT);
                }

                var oauthScopes = (await _oauthScopeRepository.FindOAuthScopesByNames(scopes)).Cast<OpenIdScope>();
                var payload = new JwsPayload
                {
                    { UserClaims.Subject, user.Id },
                };
                IdTokenBuilder.EnrichWithScopeParameter(payload, oauthScopes, user);
                IdTokenBuilder.EnrichWithClaimsParameter(payload, claims, user, authTime, AuthorizationRequestClaimTypes.UserInfo);
                foreach (var claimsSource in _claimsSources)
                {
                    await claimsSource.Enrich(payload, oauthClient).ConfigureAwait(false);
                }

                string contentType = "application/json";
                var result = JsonConvert.SerializeObject(payload).ToString();
                if (!string.IsNullOrWhiteSpace(oauthClient.UserInfoSignedResponseAlg))
                {
                    result = await _jwtBuilder.BuildClientToken(oauthClient, payload, oauthClient.UserInfoSignedResponseAlg, oauthClient.UserInfoEncryptedResponseAlg, oauthClient.UserInfoEncryptedResponseEnc);
                    contentType = "application/jwt";
                }

                return new ContentResult
                {
                    Content = result,
                    ContentType = contentType
                };
            }
            catch(OAuthException ex)
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

        private async Task<JwsPayload> Extract(string accessToken)
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
                jws = await _jwtParser.Decrypt(accessToken);
            }

            return await _jwtParser.Unsign(jws);
        }

        public string ExtractAccessToken()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_TOKEN);
            }

            foreach(var authorizationValue in Request.Headers["Authorization"])
            {
                var at = ExtractAccessToken(authorizationValue);
                if (!string.IsNullOrWhiteSpace(at))
                {
                    return at;
                }
            }

            throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_TOKEN);
        }

        private static string ExtractAccessToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var splitted = value.Split(' ');
            if (splitted.Count() != 2 || splitted.First() != "Bearer")
            {
                return null;
            }

            return splitted.Last();
        }
    }
}