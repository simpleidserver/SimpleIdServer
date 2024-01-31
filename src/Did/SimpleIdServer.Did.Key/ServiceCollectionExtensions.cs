// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Did.Crypto.Multicodec;

namespace SimpleIdServer.Did.Key;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDidKey(this IServiceCollection services)
    {
        services.AddSingleton(new DidKeyOptions());
        services.AddSingleton(MulticodecSerializerFactory.Build());
        services.AddTransient<IDidResolver, DidKeyResolver>();
        return services;
    }
}
