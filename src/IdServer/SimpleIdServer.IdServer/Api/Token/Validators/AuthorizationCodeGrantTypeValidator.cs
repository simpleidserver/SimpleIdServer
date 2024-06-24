// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IAuthorizationCodeGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class AuthorizationCodeGrantTypeValidator : IAuthorizationCodeGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Code))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.Code));
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            OpenIdCredentialValidator.ValidateOpenIdCredential(authDetails);
        }
    }
}