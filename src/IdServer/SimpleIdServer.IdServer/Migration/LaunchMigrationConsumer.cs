// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migration;

public class LaunchMigrationConsumer : 
    IConsumer<LaunchMigrationCommand>,
    IConsumer<MigrateApiScopesCommand>,
    IConsumer<MigrateIdentityScopesCommand>,
    IConsumer<MigrateApiResourcesCommand>,
    IConsumer<MigrateClientsCommand>,
    IConsumer<MigrateGroupsCommand>,
    IConsumer<MigrateUsersCommand>
{
    private readonly IConfiguration _configuration;
    private readonly IMigrationStore _migrationStore;
    private readonly IMigrationServiceFactory _migrationServiceFactory;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    public static string QueueName = "launch-migration";

    public LaunchMigrationConsumer(
        IConfiguration configuration,
        IMigrationStore migrationStore,
        IMigrationServiceFactory migrationServiceFactory,
        ITransactionBuilder transactionBuilder,
        IDateTimeHelper dateTimeHelper,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _migrationStore = migrationStore;
        _migrationServiceFactory = migrationServiceFactory;
        _transactionBuilder = transactionBuilder;
        _dateTimeHelper = dateTimeHelper;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }
    public async Task Consume(ConsumeContext<LaunchMigrationCommand> context)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var msg = context.Message;
            var migrationExecution = await _migrationStore.Get(msg.Realm, msg.Name, context.CancellationToken);
            if (migrationExecution != null)
            {
                migrationExecution = new MigrationExecution
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = msg.Name
                };
                _migrationStore.Add(migrationExecution);
            }

            await transaction.Commit(context.CancellationToken);
        }
    }

    public async Task Consume(ConsumeContext<MigrateApiScopesCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                await _groupRepository.BulkAdd(groups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            context.CancellationToken);
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

    public async Task Consume(ConsumeContext<MigrateIdentityScopesCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                await _groupRepository.BulkAdd(groups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            context.CancellationToken);
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

    public async Task Consume(ConsumeContext<MigrateApiResourcesCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                await _groupRepository.BulkAdd(groups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            context.CancellationToken);
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

    public async Task Consume(ConsumeContext<MigrateClientsCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                await _groupRepository.BulkAdd(groups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            context.CancellationToken);
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

    public async Task Consume(ConsumeContext<MigrateGroupsCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                await _groupRepository.BulkAdd(groups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            context.CancellationToken);
        if(isMigrated)
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
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsUsersMigrated,
            migrationService.NbUsers,
            async (e, c) =>
            {
                var users = await migrationService.ExtractUsers(e, c);
                await _userRepository.BulkAdd(users);
            },
            (m, s, e, n) => m.MigrateUsers(s, e, n),
            context.CancellationToken);
    }

    private async Task<bool> Migrate(
        string realm,
        string name, 
        Func<MigrationExecution, bool> isMigrated,
        Func<CancellationToken, Task<int>> nbRecordsFn,
        Func<ExtractParameter, CancellationToken, Task> importRecordsFn,
        Action<MigrationExecution, DateTime, DateTime, int> endCb,
        CancellationToken cancellationToken)
    {
        var migrationService = _migrationServiceFactory.Create(name);
        using (var transaction = _transactionBuilder.Build())
        {
            var start = _dateTimeHelper.GetCurrent();
            var options = GetOptions();
            var migrationExecution = await _migrationStore.Get(realm, name, cancellationToken);
            if (isMigrated(migrationExecution))
            {
                return true;
            }

            var nbRecords = await nbRecordsFn(cancellationToken);
            var nbPages = (int)Math.Ceiling((double)nbRecords / options.PageSize);
            var allPages = Enumerable.Range(1, nbPages);
            foreach (var page in allPages)
            {
                await importRecordsFn(new ExtractParameter
                {
                    Count = options.PageSize,
                    StartIndex = page - 1,
                }, cancellationToken);
            }

            var end = _dateTimeHelper.GetCurrent();
            endCb(migrationExecution, start, end, nbRecords),
            await transaction.Commit(cancellationToken);
        }

        return true;
    }

    private MigrationOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(MigrationOptions).Name);
        return section.Get<MigrationOptions>() ?? new MigrationOptions();
    }
}
