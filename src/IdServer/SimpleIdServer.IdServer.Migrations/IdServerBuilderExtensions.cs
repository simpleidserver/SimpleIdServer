// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Migrations.DataSeeder;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder EnableMigration(this IdServerBuilder idserverBuilder)
    {
        idserverBuilder.Services.AddTransient<IMigrationServiceFactory, MigrationServiceFactory>();
        idserverBuilder.Services.AddTransient<IDataSeeder, AddMigrationsScopeDataSeeder>();
        idserverBuilder.Services.AddTransient<ILaunchMigrationService, LaunchMigrationService>();
        idserverBuilder.Services.AddTransient<ILaunchAllMigrationsService, LaunchAllMigrationsService>();
        idserverBuilder.AddRoute("getAllDefsMigrations", Constants.Endpoints.MigDefinitions, new { controller = "Migrations", action = "GetAllDefinitions" });
        idserverBuilder.AddRoute("getAllExecutionsMigrations", Constants.Endpoints.MigExecutions, new { controller = "Migrations", action = "GetAllExecutions" });
        idserverBuilder.AddRoute("launchMigration", Constants.Endpoints.LaunchMigration, new { controller = "Migrations", action = "Launch" });
        return idserverBuilder;
    }
}
