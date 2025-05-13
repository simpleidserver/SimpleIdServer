// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Migrations;

namespace MassTransit;

public static class BusRegistrationConfiguratorExtensions
{
    public static void ConfigureMigration(this IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        busRegistrationConfigurator.AddConsumer<LaunchMigrationFaultConsumer>();
        busRegistrationConfigurator.AddConsumer<LaunchMigrationConsumer, LaunchMigrationConsumerDefinition>();
    }
}
