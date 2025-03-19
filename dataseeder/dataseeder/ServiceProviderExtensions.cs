// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static void SeedData(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dataMigrationService = scope.ServiceProvider.GetService<IDataMigrationService>();
            dataMigrationService.MigrateBeforeDeployment(CancellationToken.None).Wait();
            dataMigrationService.Migrate(CancellationToken.None).Wait();
            dataMigrationService.MigrateAfterDeployment(CancellationToken.None).Wait();
        }
    }
}
