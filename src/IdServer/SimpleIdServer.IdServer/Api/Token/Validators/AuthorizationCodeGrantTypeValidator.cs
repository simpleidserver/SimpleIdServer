// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
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
        private readonly IdServerHostOptions _options;

        public AuthorizationCodeGrantTypeValidator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Code))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.Code));
            if (_options.Type == IdServerTypes.STANDARD && string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.RedirectUri))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.RedirectUri));
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            OpenIdCredentialValidator.ValidateOpenIdCredential(authDetails);
        }
    }
}