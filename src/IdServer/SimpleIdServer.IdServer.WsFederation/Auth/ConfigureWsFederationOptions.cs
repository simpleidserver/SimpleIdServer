// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFederation;

namespace SimpleIdServer.IdServer.WsFederation.Auth
{
    public class ConfigureWsFederationOptions : IPostConfigureOptions<WsFederationOptions>
    {
        public void PostConfigure(string name, WsFederationOptions options)
        {
            options.ConfigurationManager = new IdServerWsFederationConfigurationManager(options.MetadataAddress, new WsFederationConfigurationRetriever(),
                new HttpDocumentRetriever(options.Backchannel) { RequireHttps = options.RequireHttpsMetadata });
        }
    }
}
