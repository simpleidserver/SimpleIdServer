// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Api.Token.Validators
{
    public interface IClientCredentialsGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class ClientCredentialsGrantTypeValidator : IClientCredentialsGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Scope)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "scope"));
            }
        }
    }
}