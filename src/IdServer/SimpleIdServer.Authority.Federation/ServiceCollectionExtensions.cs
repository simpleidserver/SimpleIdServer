// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Authority.Federation.Builders;

namespace SimpleIdServer.Authority.Federation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorityFederation(this IServiceCollection services, Action<AuthorityFederationOptions> authorityOptions)
    {
        services.Configure(authorityOptions);
        services.AddTransient<IAuthorityFederationEntityBuilder, AuthorityFederationEntityBuilder>();
        return services;
    }
}