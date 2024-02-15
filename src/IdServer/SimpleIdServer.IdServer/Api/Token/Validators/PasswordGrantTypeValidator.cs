// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using System.Text.Json.Nodes;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IPasswordGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class PasswordGrantTypeValidator : IPasswordGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Username)))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.Username));
            
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.Password)))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.Password));
        }
    }
}
