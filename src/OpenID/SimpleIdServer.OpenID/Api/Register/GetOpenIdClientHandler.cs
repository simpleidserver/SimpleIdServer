// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;

namespace SimpleIdServer.OpenID.Api.Register
{
    public class GetOpenIdClientHandler : GetOAuthClientHandler
    {
        public GetOpenIdClientHandler(IOAuthClientRepository oauthClientRepository, ILogger<GetOAuthClientHandler> logger) : base(oauthClientRepository, logger)
        {
        }

        protected override JObject BuildResponse(BaseClient oauthClient, string issuer)
        {
            var openidClient = oauthClient as OpenIdClient;
            return openidClient.ToDto(issuer);
        }
    }
}
