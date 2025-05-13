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
            if (migrationExecution == null)
            {
                migrationExecution = new MigrationExecution
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = msg.Name,
                    Realm = msg.Realm
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
                var allApiResourceNames = apiResources.Select(s => s.Name).ToList();
                var existingApiResources = await _apiResourceRepository.GetByNames(allApiResourceNames, context.CancellationToken);
                foreach (var existingApiResource in existingApiResources)
                {
                    if(!existingApiResource.Realms.Any(r => r.Name == msg.Realm))
                    {
                        existingApiResource.Realms.Add(currentRealm);
                        _apiResourceRepository.Update(existingApiResource);
                    }
                }

                var existingApiResourceNames = existingApiResources.Select(s => s.Name).ToList();
                var unknownApiResources = apiResources.Where(s => !existingApiResourceNames.Contains(s.Name)).ToList();
                foreach(var unknownApiResource in unknownApiResources)
                {
                    unknownApiResource.Realms = new List<Realm>
                    {
                        currentRealm
                    };
                }

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
                var allClientIds = clients.Select(s => s.ClientId).ToList();
                var existingClients = await _clientRepository.GetByClientIds(allClientIds, context.CancellationToken);
                foreach (var existingClient in existingClients)
                {
                    if (!existingClient.Realms.Any(r => r.Name == msg.Realm))
                    {
                        existingClient.Realms.Add(currentRealm);
                        _clientRepository.Update(existingClient);
                    }
                }

                var existingClientIds = existingClients.Select(s => s.ClientId).ToList();
                var unknownClients = clients.Where(s => !existingClientIds.Contains(s.ClientId)).ToList();
                foreach (var unknownClient in unknownClients)
                {
                    unknownClient.Realms = new List<Realm>
                    {
                        currentRealm
                    };
                }

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
        var currentRealm = await _realmRepository.Get(msg.Realm, context.CancellationToken);
        var isMigrated = await Migrate(
            msg.Realm,
            msg.Name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                var allGroupNames = groups.Select(s => s.Name).ToList();
                var existingGroups = await _groupRepository.GetByNames(allGroupNames, context.CancellationToken);
                foreach (var existingGroup in existingGroups)
                {
                    if (!existingGroup.Realms.Any(r => r.RealmsName == msg.Realm))
                    {
                        existingGroup.Realms.Add(new GroupRealm
                        {
                            RealmsName = msg.Name
                        });
                        _groupRepository.Update(existingGroup);
                    }
                }

                var existingGroupNames = existingGroups.Select(s => s.Name).ToList();
                var unknownGroups = groups.Where(s => !existingGroupNames.Contains(s.Name)).ToList();
                foreach(var unknownGroup in unknownGroups)
                {
                    unknownGroup.Realms = new List<GroupRealm>
                    {
                        new GroupRealm
                        {
                            RealmsName = msg.Name
                        }
                    };
                }

                await _groupRepository.BulkAdd(unknownGroups);
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
                var allUserIds = users.Select(s => s.Name).ToList();
                var existingUsers = await _userRepository.GetUsersBySubjects(allUserIds, context.CancellationToken);
                foreach (var existingUser in existingUsers)
                {
                    if (!existingUser.Realms.Any(r => r.RealmsName == msg.Realm))
                    {
                        existingUser.Realms.Add(new RealmUser
                        {
                            RealmsName = msg.Name
                        });
                        _userRepository.Update(existingUser);
                    }
                }

                var existingUserNames = existingUsers.Select(s => s.Name).ToList();
                var unknownUsers = users.Where(s => !existingUserNames.Contains(s.Name)).ToList();
                foreach(var unknownUser in unknownUsers)
                {
                    unknownUser.Realms = new List<RealmUser>
                    {
                        new RealmUser
                        {
                            RealmsName = msg.Realm
                        }
                    };
                }

                await _userRepository.BulkAdd(unknownUsers);
            },
            (m, s, e, n) => m.MigrateUsers(s, e, n),
            context.CancellationToken);
    }

    private async Task MigrateScopes(List<Scope> scopes, Domains.Realm realm, IMigrationService migrationService, CancellationToken cancellationToken)
    {
        var allScopeNames = scopes.Select(s => s.Name).ToList();
        var existingScopes = await _scopeRepository.GetByNames(allScopeNames, cancellationToken);
        foreach (var existingScope in existingScopes)
        {
            if(!existingScope.Realms.Any(r => r.Name == realm.Name))
            {
                existingScope.Realms.Add(realm);
                _scopeRepository.Update(existingScope);
            }
        }

        var existingScopeNames = existingScopes.Select(s => s.Name).ToList();
        var unknownScopes = scopes.Where(s => !existingScopeNames.Contains(s.Name)).ToList();
        foreach(var unknownScope in unknownScopes)
        {
            unknownScope.Realms = new List<Realm>
            {
                realm
            };
        }

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