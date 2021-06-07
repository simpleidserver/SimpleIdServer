// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Management.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Management
{
    public class UpdateOpenIdClientHandler : UpdateOAuthClientHandler
    {
        public UpdateOpenIdClientHandler(
            IOAuthClientRepository oauthClientRepository,
            IOAuthScopeRepository oauthScopeRepository,
            ILogger<UpdateOpenIdClientHandler> logger) : base(oauthClientRepository, oauthScopeRepository, logger) { }


        protected override async Task UpdateClient(BaseClient oauthClient, BaseClient extractClient, CancellationToken cancellationToken)
        {
            await base.UpdateClient(oauthClient, extractClient, cancellationToken);
            var openidClient = (OpenIdClient)oauthClient;
            var openidExtractedClient = (OpenIdClient)extractClient;
            openidClient.ApplicationKind = openidExtractedClient.ApplicationKind;
            openidClient.PostLogoutRedirectUris = openidExtractedClient.PostLogoutRedirectUris;
        }

        protected override BaseClient ExtractClient(JObject content)
        {
            var result = content.ToDomain();
            return result;
        }
    }
}