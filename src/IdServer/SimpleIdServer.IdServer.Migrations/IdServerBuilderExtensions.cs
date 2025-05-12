// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Migrations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder EnableMigration(this IdServerBuilder idserverBuilder)
    {
        idserverBuilder.Services.AddTransient<IMigrationServiceFactory, MigrationServiceFactory>();
        idserverBuilder.AddRoute("getAllDefsMigrations", SimpleIdServer.IdServer.Migrations.Constants.Endpoints.MigDefinitions, new { controller = "Migrations", action = "GetAllDefinitions" });
        idserverBuilder.AddRoute("getAllExecutionsMigrations", SimpleIdServer.IdServer.Migrations.Constants.Endpoints.MigExecutions, new { controller = "Migrations", action = "GetAllExecutions" });
        idserverBuilder.AddRoute("launchMigration", SimpleIdServer.IdServer.Migrations.Constants.Endpoints.LaunchMigration, new { controller = "Migrations", action = "Launch" });
        return idserverBuilder;
    }
}
