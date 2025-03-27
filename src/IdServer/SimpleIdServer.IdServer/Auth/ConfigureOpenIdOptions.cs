// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;

namespace SimpleIdServer.IdServer.Auth;


public class ConfigureOpenIdOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ConfigureOpenIdOptions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void PostConfigure(string name, OpenIdConnectOptions options)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
            return true; 
        };
        var httpClient = new HttpClient(handler);
        options.Backchannel = httpClient;
        options.ConfigurationManager = new IdServerOpenIdConfigurationManager(_serviceProvider, options.Authority, new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever(options.Backchannel) { RequireHttps = options.RequireHttpsMetadata })
        {
            RefreshInterval = options.RefreshInterval,
            AutomaticRefreshInterval = options.AutomaticRefreshInterval,
        };
    }
}