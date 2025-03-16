// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataseeder(this IServiceCollection services)
    {
        services.AddTransient<IDataMigrationService, DataMigrationService>();
        return services;
    }
}