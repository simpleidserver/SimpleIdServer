// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SimpleIdServer.IdServer.Auth
{

    public class ConfigureOpenIdOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        public void PostConfigure(string name, OpenIdConnectOptions options)
        {
            options.ConfigurationManager = new IdServerConfigurationManager(options.Authority, new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(options.Backchannel) { RequireHttps = options.RequireHttpsMetadata })
            {
                RefreshInterval = options.RefreshInterval,
                AutomaticRefreshInterval = options.AutomaticRefreshInterval,
            };
        }
    }
}
