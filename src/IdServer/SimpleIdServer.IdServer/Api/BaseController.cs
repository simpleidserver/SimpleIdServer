// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api
{
    public class BaseController : Controller
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IJwtBuilder _jwtBuilder;

        public BaseController(ITokenRepository tokenRepository, IJwtBuilder jwtBuilder)
        {
            _tokenRepository = tokenRepository;
            _jwtBuilder = jwtBuilder;
        }

        protected IJwtBuilder JwtBuilder => _jwtBuilder;

        protected string ExtractBearerToken()
        {
            if (TryExtractBearerToken(out string result)) return result;
            throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, Global.MissingToken);
        }

        protected bool TryExtractBearerToken(out string bearerToken)
        {
            bearerToken = null;
            if (Request.Headers.ContainsKey(Constants.AuthorizationHeaderName))
            {
                foreach (var authorizationValue in Request.Headers[Constants.AuthorizationHeaderName])
                {
                    var at = authorizationValue.ExtractAuthorizationValue(new string[] { AutenticationSchemes.Bearer });
                    if (!string.IsNullOrWhiteSpace(at))
                    {
                        bearerToken = at;
                        return true;
                    }
                }
            }

            return false;
        }

        protected Task<(JsonWebToken, Domains.Token)> CheckHasPAT(string realm) => CheckAccessToken(realm, Config.DefaultScopes.UmaProtection.Name);

        protected async Task<(JsonWebToken, Domains.Token)> CheckAccessToken(string realm, string scope = null, string accessToken = null)
        {
            if(string.IsNullOrWhiteSpace(accessToken))
                accessToken = ExtractBearerToken();

            var handler = new JsonWebTokenHandler();
            JsonWebToken jwt = null;
            if(handler.CanReadToken(accessToken))
            {
                var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(realm, accessToken);
                if (extractionResult.Error != null)
                    throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_REQUEST, extractionResult.Error);
                if(!string.IsNullOrWhiteSpace(scope) && !extractionResult.Jwt.Claims.Any(c => c.Type == "scope" && c.Value == scope))
                {
                    throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.REQUEST_DENIED, Global.UnauthorizedAccessPermissionApi);
                }

                jwt = extractionResult.Jwt;
            }

            var token = await _tokenRepository.Get(accessToken, CancellationToken.None);
            if (token == null || token.IsExpired()) throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.UnknownAccessToken);
            return (jwt ?? handler.ReadJsonWebToken(token.Data), token);
        }

        protected bool TryGetIdentityToken(string realm, out JsonWebToken jsonWebToken)
        {
            jsonWebToken = null;
            string bearerToken;
            if (TryExtractBearerToken(out bearerToken)) return false;
            if (string.IsNullOrWhiteSpace(bearerToken)) return false;
            var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(realm, bearerToken);
            if (extractionResult.Error != null)
                return false;
            jsonWebToken = extractionResult.Jwt;
            return true;
        }

        protected ContentResult BuildError(OAuthException ex) => BuildError(ex.StatusCode ?? HttpStatusCode.InternalServerError, ex.Code, ex.Message);

        protected ContentResult BuildError(Exception ex) => BuildError(HttpStatusCode.InternalServerError, ErrorCodes.UNEXPECTED_ERROR, ex.Message);

        protected ContentResult BuildError(HttpStatusCode statusCode, string error, string errorDescription) => new ContentResult
        {
            StatusCode = (int)statusCode,
            Content = new JsonObject
            {
                [ErrorResponseParameters.Error] = error,
                [ErrorResponseParameters.ErrorDescription] = errorDescription
            }.ToJsonString(),
            ContentType = "application/json"
        };
    }
}
