// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace EfdataSeeder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfdataSeeder(this IServiceCollection services)
    {
        services.AddTransient<IDataMigrationService, DataMigrationService>();
        return services;
    }
}