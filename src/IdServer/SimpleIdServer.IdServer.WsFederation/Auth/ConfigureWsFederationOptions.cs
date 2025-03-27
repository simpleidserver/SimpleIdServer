// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFederation;

namespace SimpleIdServer.IdServer.WsFederation.Auth;

public class ConfigureWsFederationOptions : IPostConfigureOptions<WsFederationOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ConfigureWsFederationOptions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void PostConfigure(string name, WsFederationOptions options)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
            return true;
        };
        var httpClient = new HttpClient(handler);
        options.Backchannel = httpClient;
        options.ConfigurationManager = new IdServerWsFederationConfigurationManager(_serviceProvider, options.MetadataAddress, new WsFederationConfigurationRetriever(),
            new HttpDocumentRetriever(options.Backchannel) { RequireHttps = options.RequireHttpsMetadata });
    }
}
