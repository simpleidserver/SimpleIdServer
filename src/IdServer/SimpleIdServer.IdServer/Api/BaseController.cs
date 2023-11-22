// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using System;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api
{
    public class BaseController : Controller
    {
        protected string ExtractBearerToken()
        {
            if (TryExtractBearerToken(out string result)) return result;
            throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, ErrorMessages.MISSING_TOKEN);
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

        protected void CheckHasPAT(string realm, IJwtBuilder jwtBuilder) => CheckAccessToken(realm, Constants.StandardScopes.UmaProtection.Name, jwtBuilder);

        protected void CheckAccessToken(string realm, string scope, IJwtBuilder jwtBuilder)
        {
            var bearerToken = ExtractBearerToken();
            var extractionResult = jwtBuilder.ReadSelfIssuedJsonWebToken(realm, bearerToken);
            if (extractionResult.Error != null)
                throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_REQUEST, extractionResult.Error);
            if (!extractionResult.Jwt.Claims.Any(c => c.Type == "scope" && c.Value == scope))
                throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.REQUEST_DENIED, ErrorMessages.UNAUTHORIZED_ACCESS_PERMISSION_API);
        }

        protected bool TryGetIdentityToken(string realm, IJwtBuilder jwtBuilder, out JsonWebToken jsonWebToken)
        {
            jsonWebToken = null;
            string bearerToken;
            if (TryExtractBearerToken(out bearerToken)) return false;
            if (string.IsNullOrWhiteSpace(bearerToken)) return false;
            var extractionResult = jwtBuilder.ReadSelfIssuedJsonWebToken(realm, bearerToken);
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
