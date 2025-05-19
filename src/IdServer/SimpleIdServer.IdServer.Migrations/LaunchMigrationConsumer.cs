// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;

namespace SimpleIdServer.IdServer.Migrations;

public class LaunchMigrationConsumer : 
    IConsumer<LaunchMigrationCommand>,
    IConsumer<MigrateApiScopesCommand>,
    IConsumer<MigrateIdentityScopesCommand>,
    IConsumer<MigrateApiResourcesCommand>,
    IConsumer<MigrateClientsCommand>,
    IConsumer<MigrateGroupsCommand>,
    IConsumer<MigrateUsersCommand>
{
    private readonly ILaunchMigrationService _launchMigrationService;
    public static string QueueName = "launch-migration";

    public LaunchMigrationConsumer(
        ILaunchMigrationService launchMigrationService)
    {
        _launchMigrationService = launchMigrationService;
    }
    public async Task Consume(ConsumeContext<LaunchMigrationCommand> context)
    {
        var msg = context.Message;
        await _launchMigrationService.Start(msg.Realm, msg.Name, context.CancellationToken);
        var destination = new Uri($"queue:{QueueName}");
        await context.Send(destination, new MigrateApiScopesCommand
        {
            Name = msg.Name,
            Realm = msg.Realm
        });
    }

    public async Task Consume(ConsumeContext<MigrateApiScopesCommand> context)
    {
        var msg = context.Message;
        var isMigrated = await _launchMigrationService.MigrateApiScopes(msg.Realm, msg.Name, context.CancellationToken);
        if (isMigrated)
        {
            var destination = new Uri($"queue:{QueueName}");
            await context.Send(destination, new MigrateIdentityScopesCommand
            {
                Name = msg.Name,
                Realm = msg.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<MigrateIdentityScopesCommand> context)
    {
        var msg = context.Message;
        var isMigrated = await _launchMigrationService.MigrateIdentityScopes(msg.Realm, msg.Name, context.CancellationToken);
        if (isMigrated)
        {
            var destination = new Uri($"queue:{QueueName}");
            await context.Send(destination, new MigrateApiResourcesCommand
            {
                Name = msg.Name,
                Realm = msg.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<MigrateApiResourcesCommand> context)
    {
        var msg = context.Message;
        var isMigrated = await _launchMigrationService.MigrateApiResources(msg.Realm, msg.Name, context.CancellationToken);
        if (isMigrated)
        {
            var destination = new Uri($"queue:{QueueName}");
            await context.Send(destination, new MigrateClientsCommand
            {
                Name = msg.Name,
                Realm = msg.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<MigrateClientsCommand> context)
    {
        var msg = context.Message;
        var isMigrated = await _launchMigrationService.MigrateClients(msg.Realm, msg.Name, context.CancellationToken);
        if (isMigrated)
        {
            var destination = new Uri($"queue:{QueueName}");
            await context.Send(destination, new MigrateGroupsCommand
            {
                Name = msg.Name,
                Realm = msg.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<MigrateGroupsCommand> context)
    {
        var msg = context.Message;
        var isMigrated = await _launchMigrationService.MigrateGroups(msg.Realm, msg.Name, context.CancellationToken);
        if (isMigrated)
        {
            var destination = new Uri($"queue:{QueueName}");
            await context.Send(destination, new MigrateUsersCommand
            {
                Name = msg.Name,
                Realm = msg.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<MigrateUsersCommand> context)
    {
        var msg = context.Message;
        await _launchMigrationService.MigrateUsers(msg.Realm, msg.Name, context.CancellationToken);
    }
}

public class LaunchMigrationConsumerDefinition : ConsumerDefinition<LaunchMigrationConsumer>
{
    public LaunchMigrationConsumerDefinition()
    {
        EndpointName = LaunchMigrationConsumer.QueueName;
        ConcurrentMessageLimit = 8;
    }
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<LaunchMigrationConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}