// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Authority.Federation;
using SimpleIdServer.Authority.Federation.Builders;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorityFederation(this IServiceCollection services, Action<AuthorityFederationOptions> authorityOptions)
    {
        services.Configure(authorityOptions);
        services.AddTransient<IAuthorityFederationEntityBuilder, AuthorityFederationEntityBuilder>();
        services.AddTransient<SimpleIdServer.IdServer.Helpers.IHttpClientFactory, SimpleIdServer.IdServer.Helpers.HttpClientFactory>();
        return services;
    }
}