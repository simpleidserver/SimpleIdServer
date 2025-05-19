// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Migrations;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static void LaunchMigration(this IServiceProvider services, string realm = SimpleIdServer.IdServer.Constants.DefaultRealm)
    {
        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var migrationService = scope.ServiceProvider.GetRequiredService<ILaunchAllMigrationsService>();
            migrationService.LaunchAll(realm, CancellationToken.None).Wait();
        }
    }
}
