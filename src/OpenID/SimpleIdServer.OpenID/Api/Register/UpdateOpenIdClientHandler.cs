// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Extensions;

namespace SimpleIdServer.OpenID.Api.Register
{
    public class UpdateOpenIdClientHandler : UpdateOAuthClientHandler
    {
        public UpdateOpenIdClientHandler(IOAuthClientValidator oauthClientValidator, IOAuthClientRepository oauthClientRepository, ILogger<UpdateOAuthClientHandler> logger) : base(oauthClientValidator, oauthClientRepository, logger)
        {
        }

        protected override BaseClient ExtractClient(HandlerContext handlerContext)
        {
            var result = handlerContext.Request.RequestData.ToDomain();
            return result;
        }
    }
}
