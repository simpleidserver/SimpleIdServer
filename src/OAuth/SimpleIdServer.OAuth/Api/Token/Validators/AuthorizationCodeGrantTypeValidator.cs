// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;

namespace SimpleIdServer.OAuth.Api.Token.Validators
{
    public interface IAuthorizationCodeGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class AuthorizationCodeGrantTypeValidator : IAuthorizationCodeGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.HttpBody.GetStr(TokenRequestParameters.Code)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.Code));
            }

            if (string.IsNullOrWhiteSpace(context.Request.HttpBody.GetStr(TokenRequestParameters.RedirectUri)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.RedirectUri));
            }
        }
    }
}