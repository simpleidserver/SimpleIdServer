// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IRevokeTokenValidator
    {
        RevokeTokenValidationResult Validate(JsonObject jObjBody);
    }

    public class RevokeTokenValidationResult
    {
        public RevokeTokenValidationResult(string token, string tokenTypeHint)
        {
            Token = token;
            TokenTypeHint = tokenTypeHint;
        }

        public string Token { get; set; }
        public string TokenTypeHint { get; set; }
    }

    public class RevokeTokenValidator : IRevokeTokenValidator
    {
        public RevokeTokenValidationResult Validate(JsonObject jObjBody)
        {
            var token = jObjBody.GetStr(RevokeTokenRequestParameters.Token);
            if (string.IsNullOrWhiteSpace(token))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RevokeTokenRequestParameters.Token));

            var tokenTypeHint = jObjBody.GetStr(RevokeTokenRequestParameters.TokenTypeHint);
            var supportedTokens = GetSupportedTokens();
            if (!string.IsNullOrWhiteSpace(tokenTypeHint) && !supportedTokens.Contains(tokenTypeHint))
                throw new OAuthException(ErrorCodes.UNSUPPORTED_TOKEN_TYPE, string.Format(Global.UnknownTokenTypeHint, tokenTypeHint));

            return new RevokeTokenValidationResult(token, tokenTypeHint);
        }

        protected virtual IEnumerable<string> GetSupportedTokens()
        {
            return new string[]
            {
                TokenResponseParameters.AccessToken,
                TokenResponseParameters.RefreshToken
            };
        }
    }
}