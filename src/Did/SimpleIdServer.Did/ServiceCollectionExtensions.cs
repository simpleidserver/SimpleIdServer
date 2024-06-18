// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDid(this IServiceCollection services)
    {
        services.AddTransient<IDidFactoryResolver, DidFactoryResolver>();
        services.AddTransient<IJwtVerifier, JwtVerifier>();
        return services;
    }
}
