// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.IdentityProvider;
using SimpleIdServer.FastFed.IdentityProvider.Apis.FastFed;
using SimpleIdServer.FastFed.IdentityProvider.Resolvers;
using SimpleIdServer.IdServer.Helpers;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFastFedIdentityProvider(this IServiceCollection services, Action<FastFedIdentityProviderOptions> callback = null)
    {
        if (callback == null) services.Configure<FastFedIdentityProviderOptions>(o => { });
        else services.Configure(callback);
        services.AddHttpContextAccessor();
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddTransient<IIssuerResolver, IssuerResolver>();
        services.AddTransient<IGetIdentityProviderMetadataQuery, GetIdentityProviderMetadataQuery>();
        return services;
    }
}
