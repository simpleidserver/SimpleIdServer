// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IPreAuthorizedCodeGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class PreAuthorizedCodeGrantTypeValidator : IPreAuthorizedCodeGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            var preAuthorizedContext = context.Request.RequestData.GetPreAuthorizedCode();
            if(string.IsNullOrWhiteSpace(preAuthorizedContext)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.PreAuthorizedCode));
        }
    }
}
