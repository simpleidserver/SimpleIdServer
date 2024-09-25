// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.Client;
using SimpleIdServer.FastFed.Resolvers;
using SimpleIdServer.IdServer.Helpers;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static FastFedServicesBuilder AddFastFed(this IServiceCollection services, Action<FastFedOptions> cb = null)
    {
        if (cb == null) services.Configure<FastFedOptions>(o => { });
        else services.Configure(cb);
        services.AddHttpContextAccessor();
        services.AddTransient<IIssuerResolver, IssuerResolver>();
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddTransient<IFastFedClientFactory, FastFedClientFactory>();
        services.AddTransient<IGetProviderMetadataQuery, GetProviderMetadataQuery>();
        return new FastFedServicesBuilder(services);
    }
}
