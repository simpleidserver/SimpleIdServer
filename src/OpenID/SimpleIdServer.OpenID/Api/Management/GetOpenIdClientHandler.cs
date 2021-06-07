// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Management.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;

namespace SimpleIdServer.OpenID.Api.Management
{
    public class GetOpenIdClientHandler : GetOAuthClientHandler
    {
        public GetOpenIdClientHandler(IOAuthClientRepository oauthClientRepository) : base(oauthClientRepository)
        {
        }

        protected override JObject ToDto(BaseClient client, string issuer)
        {
            return (client as OpenIdClient).ToDto(issuer);
        }
    }
}
