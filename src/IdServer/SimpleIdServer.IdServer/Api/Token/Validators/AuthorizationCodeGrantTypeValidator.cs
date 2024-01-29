// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using System.Collections.Generic;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Code))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.Code));
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.RedirectUri))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.RedirectUri));
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            ValidateOpenIdCredential(authDetails);
        }

        private void ValidateOpenIdCredential(ICollection<AuthorizationData> authDetails)
        {
            var openidCredentials = authDetails.Where(t => t.Type == Constants.StandardAuthorizationDetails.OpenIdCredential);
            if (!openidCredentials.Any()) return;
            foreach(var openidCredential in openidCredentials)
                if (!openidCredential.AdditionalData.ContainsKey(Constants.StandardAuthorizationDetails.CredentialConfigurationId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, Constants.StandardAuthorizationDetails.CredentialConfigurationId));
        }
    }
}