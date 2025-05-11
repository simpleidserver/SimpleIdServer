// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migration;

public class LaunchMigrationConsumer : 
    IConsumer<LaunchMigrationCommand>,
    IConsumer<MigrateGroupsCommand>
{
    private readonly IConfiguration _configuration;
    private readonly IMigrationStore _migrationStore;
    private readonly IMigrationServiceFactory _migrationServiceFactory;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGroupRepository _groupRepository;

    public LaunchMigrationConsumer(
        IConfiguration configuration,
        IMigrationStore migrationStore,
        IMigrationServiceFactory migrationServiceFactory,
        ITransactionBuilder transactionBuilder,
        IDateTimeHelper dateTimeHelper,
        IGroupRepository groupRepository)
    {
        _configuration = configuration;
        _migrationStore = migrationStore;
        _migrationServiceFactory = migrationServiceFactory;
        _transactionBuilder = transactionBuilder;
        _dateTimeHelper = dateTimeHelper;
        _groupRepository = groupRepository;
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

    public async Task Consume(ConsumeContext<MigrateGroupsCommand> context)
    {
        var msg = context.Message;
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        using (var transaction = _transactionBuilder.Build())
        {
            var start = _dateTimeHelper.GetCurrent();
            var options = GetOptions();
            var migrationExecution = await _migrationStore.Get(msg.Realm, msg.Name, context.CancellationToken);
            if(migrationExecution.IsGroupsMigrated)
            {
                return;            
            }

            var nbGroups = await migrationService.NbGroups(context.CancellationToken);
            var nbPages = (int)Math.Ceiling((double)nbGroups / options.PageSize);
            var allPages = Enumerable.Range(1, nbPages);
            foreach (var page in allPages)
            {
                var groups = await migrationService.ExtractGroups(new ExtractParameter
                {
                    Count = options.PageSize,
                    StartIndex = page - 1,
                }, context.CancellationToken);
                foreach(var group in groups)
                {
                    _groupRepository.Add(group);
                }
            }

            var end = _dateTimeHelper.GetCurrent();
            migrationExecution.MigrateGroups(start, end, nbGroups);
            await transaction.Commit(context.CancellationToken);
        }
    }

    private MigrationOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(MigrationOptions).Name);
        return section.Get<MigrationOptions>() ?? new MigrationOptions();
    }
}
