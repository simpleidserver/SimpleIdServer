// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

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
    private readonly IConfiguration _configuration;
    private readonly IMigrationStore _migrationStore;
    private readonly IMigrationServiceFactory _migrationServiceFactory;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    public static string QueueName = "launch-migration";

    public LaunchMigrationConsumer(
        IConfiguration configuration,
        IMigrationStore migrationStore,
        IMigrationServiceFactory migrationServiceFactory,
        ITransactionBuilder transactionBuilder,
        IDateTimeHelper dateTimeHelper,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IApiResourceRepository apiResourceRepository,
        IClientRepository clientRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _migrationStore = migrationStore;
        _migrationServiceFactory = migrationServiceFactory;
        _transactionBuilder = transactionBuilder;
        _dateTimeHelper = dateTimeHelper;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _apiResourceRepository = apiResourceRepository;
        _clientRepository = clientRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }
    public async Task Consume(ConsumeContext<LaunchMigrationCommand> context)
    {
        var msg = context.Message;
        using (var transaction = _transactionBuilder.Build())
        {
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
        var currentRealm = await _realmRepository.Get(msg.Realm, context.CancellationToken);
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsApiScopesMigrated,
            migrationService.NbApiScopes,
            async (e, c) =>
            {
                var extractedScopes = await migrationService.ExtractApiScopes(e, c);
                await MigrateScopes(extractedScopes, currentRealm, migrationService, c);
            },
            (m, s, e, n) => m.MigrateApiScopes(s, e, n),
            context.CancellationToken);
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
        var currentRealm = await _realmRepository.Get(msg.Realm, context.CancellationToken);
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsIdentityScopesMigrated,
            migrationService.NbIdentityScopes,
            async (e, c) =>
            {
                var extractedScopes = await migrationService.ExtractIdentityScopes(e, c);
                await MigrateScopes(extractedScopes, currentRealm, migrationService, c);
            },
            (m, s, e, n) => m.MigrateIdentityScopes(s, e, n),
            context.CancellationToken);
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
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var currentRealm = await _realmRepository.Get(msg.Realm, context.CancellationToken);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsApiResoucesMigrated,
            migrationService.NbApiResources,
            async (e, c) =>
            {
                var apiResources = await migrationService.ExtractApiResources(e, c);
                var allApiResourceIds = apiResources.Select(s => s.Id).ToList();
                var existingApiResources = await _apiResourceRepository.GetByIds(allApiResourceIds, context.CancellationToken);
                foreach (var existingApiResource in existingApiResources)
                {
                    existingApiResource.Realms.Add(currentRealm);
                    _apiResourceRepository.Update(existingApiResource);
                }

                var existingApiResourceIds = existingApiResources.Select(s => s.Id).ToList();
                var unknownApiResources = apiResources.Where(s => !existingApiResourceIds.Contains(s.Id)).ToList();
                await _apiResourceRepository.BulkAdd(unknownApiResources);
            },
            (m, s, e, n) => m.MigrateApiResources(s, e, n),
            context.CancellationToken);
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
        var currentRealm = await _realmRepository.Get(msg.Realm, context.CancellationToken);
        var migrationService = _migrationServiceFactory.Create(msg.Name);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsClientsMigrated,
            migrationService.NbClients,
            async (e, c) =>
            {
                var clients = await migrationService.ExtractClients(e, c);
                var allClientIds = clients.Select(s => s.Id).ToList();
                var existingClients = await _clientRepository.GetByClientIds(allClientIds, context.CancellationToken);
                foreach (var existingClient in existingClients)
                {
                    existingClient.Realms.Add(currentRealm);
                    _clientRepository.Update(existingClient);
                }

                var existingClientIds = existingClients.Select(s => s.Id).ToList();
                var unknownClients = clients.Where(s => !existingClientIds.Contains(s.Id)).ToList();
                await _clientRepository.BulkAdd(unknownClients);
            },
            (m, s, e, n) => m.MigrateClients(s, e, n),
            context.CancellationToken);
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

    private async Task MigrateScopes(List<Scope> scopes, Domains.Realm realm, IMigrationService migrationService, CancellationToken cancellationToken)
    {
        var allScopeIds = scopes.Select(s => s.Id).ToList();
        var existingScopes = await _scopeRepository.GetByIds(allScopeIds, cancellationToken);
        foreach (var existingScope in existingScopes)
        {
            existingScope.Realms.Add(realm);
            _scopeRepository.Update(existingScope);
        }

        var existingScopeIds = existingScopes.Select(s => s.Id).ToList();
        var unknownScopes = scopes.Where(s => !existingScopeIds.Contains(s.Id)).ToList();
        await _scopeRepository.BulkAdd(unknownScopes);
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
            endCb(migrationExecution, start, end, nbRecords);
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
