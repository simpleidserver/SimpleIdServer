// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.ApplicationProvider;
using SimpleIdServer.FastFed.ApplicationProvider.Apis.ProviderMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.FastFed.ApplicationProvider.Stores;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.Webfinger.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFastFedApplicationProvider(this IServiceCollection services, Action<FastFedApplicationProviderOptions> callback = null)
    {
        if (callback == null) services.Configure<FastFedApplicationProviderOptions>(o => { });
        else services.Configure(callback);
        services.AddTransient<IFastFedService, FastFedService>();
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddTransient<IIdentityProviderFederationStore, IdentityProviderFederationStore>();
        services.AddTransient<IGetApplicationProviderMetadataQuery, GetApplicationProviderMetadataQuery>();
        services.AddTransient<IWebfingerClientFactory, WebfingerClientFactory>();
        services.AddTransient<IWebfingerUrlResolver, WebfingerUrlResolver>();
        return services;
    }
}
