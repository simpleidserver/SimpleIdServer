// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;

namespace SimpleIdServer.OAuth.Api.Token.Validators
{
    public interface IRevokeTokenValidator
    {
        void Validate(JObject jObjBody);
    }

    public class RevokeTokenValidator : IRevokeTokenValidator
    {
        public void Validate(JObject jObjBody)
        {
            if (string.IsNullOrWhiteSpace(jObjBody.GetStr(RevokeTokenRequestParameters.Token)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RevokeTokenRequestParameters.Token));
            }

            var tokenTypeHint = jObjBody.GetStr(RevokeTokenRequestParameters.TokenTypeHint);
            if (!string.IsNullOrWhiteSpace(tokenTypeHint) && tokenTypeHint != "access_token" && tokenTypeHint != "refresh_token")
            {
                throw new OAuthException(ErrorCodes.UNSUPPORTED_TOKEN_TYPE, string.Format(ErrorMessages.UNKNOWN_REFRESH_TOKEN_HINT, tokenTypeHint));
            }
        }
    }
}