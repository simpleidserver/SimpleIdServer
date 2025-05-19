// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Migrations;

public interface ILaunchMigrationService
{
    Task Launch(string realm, string name, CancellationToken cancellationToken);
    Task Start(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateApiScopes(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateIdentityScopes(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateApiResources(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateClients(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateGroups(string realm, string name, CancellationToken cancellationToken);
    Task<bool> MigrateUsers(string realm, string name, CancellationToken cancellationToken);
}

public class LaunchMigrationService : ILaunchMigrationService
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IMigrationStore _migrationStore;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IMigrationServiceFactory _migrationServiceFactory;
    private readonly IConfiguration _configuration;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public LaunchMigrationService(
        ITransactionBuilder transactionBuilder,
        IMigrationStore migrationStore,
        IDateTimeHelper dateTimeHelper,
        IMigrationServiceFactory migrationServiceFactory,
        IConfiguration configuration,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IApiResourceRepository apiResourceRepository,
        IClientRepository clientRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _transactionBuilder = transactionBuilder;
        _migrationStore = migrationStore;
        _dateTimeHelper = dateTimeHelper;
        _migrationServiceFactory = migrationServiceFactory;
        _configuration = configuration;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _apiResourceRepository = apiResourceRepository;
        _clientRepository = clientRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task Launch(string realm, string name, CancellationToken cancellationToken)
    {
        await Start(realm, name, cancellationToken);
        if(!await HandleError(MigrationExecutionHistoryTypes.APISCOPES, realm, name, MigrateApiScopes, cancellationToken))
        {
            return;
        }

        if(!await HandleError(MigrationExecutionHistoryTypes.IDENTITYSCOPES, realm, name, MigrateIdentityScopes, cancellationToken))
        {
            return;
        }

        if(!await HandleError(MigrationExecutionHistoryTypes.APIRESOURCES, realm, name, MigrateApiResources, cancellationToken))
        {
            return;
        }

        if(!await HandleError(MigrationExecutionHistoryTypes.CLIENTS, realm, name, MigrateClients, cancellationToken))
        {
            return;
        }

        if (!await HandleError(MigrationExecutionHistoryTypes.GROUPS, realm, name, MigrateGroups, cancellationToken))
        {
            return;
        }

        await HandleError(MigrationExecutionHistoryTypes.USERS, realm, name, MigrateUsers, cancellationToken);
    }

    public async Task Start(string realm, string name, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var migrationExecution = await _migrationStore.Get(realm, name, cancellationToken);
            if (migrationExecution == null)
            {
                migrationExecution = new MigrationExecution
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Realm = realm
                };
                _migrationStore.Add(migrationExecution);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    public async Task<bool> MigrateApiScopes(string realm, string name, CancellationToken cancellationToken)
    {
        var currentRealm = await _realmRepository.Get(realm, cancellationToken);
        var migrationService = _migrationServiceFactory.Create(name);
        return await Migrate(
            realm,
            name,
            (m) => m.IsApiScopesMigrated,
            migrationService.NbApiScopes,
            async (e, c) =>
            {
                var extractedScopes = await migrationService.ExtractApiScopes(e, c);
                await MigrateScopes(extractedScopes, currentRealm, c);
            },
            (m, s, e, n) => m.MigrateApiScopes(s, e, n),
            cancellationToken);
    }

    public async Task<bool> MigrateIdentityScopes(string realm, string name, CancellationToken cancellationToken)
    {
        var currentRealm = await _realmRepository.Get(realm, cancellationToken);
        var migrationService = _migrationServiceFactory.Create(name);
        return await Migrate(
            realm,
            name,
            (m) => m.IsIdentityScopesMigrated,
            migrationService.NbIdentityScopes,
            async (e, c) =>
            {
                var extractedScopes = await migrationService.ExtractIdentityScopes(e, c);
                await MigrateScopes(extractedScopes, currentRealm, c);
            },
            (m, s, e, n) => m.MigrateIdentityScopes(s, e, n),
            cancellationToken);
    }

    public async Task<bool> MigrateApiResources(string realm, string name, CancellationToken cancellationToken)
    {
        var migrationService = _migrationServiceFactory.Create(name);
        var currentRealm = await _realmRepository.Get(realm, cancellationToken);
        return await Migrate(
            realm,
            name,
            (m) => m.IsApiResoucesMigrated,
            migrationService.NbApiResources,
            async (e, c) =>
            {
                var apiResources = await migrationService.ExtractApiResources(e, c);
                var allApiResourceNames = apiResources.Select(s => s.Name).ToList();
                var existingApiResources = await _apiResourceRepository.GetByNames(allApiResourceNames, cancellationToken);
                foreach (var existingApiResource in existingApiResources)
                {
                    if (!existingApiResource.Realms.Any(r => r.Name == realm))
                    {
                        existingApiResource.Realms.Add(currentRealm);
                        _apiResourceRepository.Update(existingApiResource);
                    }
                }

                var existingApiResourceNames = existingApiResources.Select(s => s.Name).ToList();
                var unknownApiResources = apiResources.Where(s => !existingApiResourceNames.Contains(s.Name)).ToList();
                foreach (var unknownApiResource in unknownApiResources)
                {
                    unknownApiResource.Realms = new List<Realm>
                    {
                        currentRealm
                    };
                }

                await _apiResourceRepository.BulkAdd(unknownApiResources);
            },
            (m, s, e, n) => m.MigrateApiResources(s, e, n),
            cancellationToken);
    }
       
    public async Task<bool> MigrateClients(string realm, string name, CancellationToken cancellationToken)
    {
        var currentRealm = await _realmRepository.Get(realm, cancellationToken);
        var migrationService = _migrationServiceFactory.Create(name);
        return await Migrate(
            realm,
            name,
            (m) => m.IsClientsMigrated,
            migrationService.NbClients,
            async (e, c) =>
            {
                var clients = await migrationService.ExtractClients(e, c);
                var allClientIds = clients.Select(s => s.ClientId).ToList();
                var existingClients = await _clientRepository.GetByClientIds(allClientIds, cancellationToken);
                foreach (var existingClient in existingClients)
                {
                    if (!existingClient.Realms.Any(r => r.Name == realm))
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
            cancellationToken);
    }

    public async Task<bool> MigrateGroups(string realm, string name, CancellationToken cancellationToken)
    {
        var migrationService = _migrationServiceFactory.Create(name);
        var currentRealm = await _realmRepository.Get(realm, cancellationToken);
        return await Migrate(
            realm,
            name,
            (m) => m.IsGroupsMigrated,
            migrationService.NbGroups,
            async (e, c) =>
            {
                var groups = await migrationService.ExtractGroups(e, c);
                var allGroupNames = groups.Select(s => s.Name).ToList();
                var existingGroups = await _groupRepository.GetByNames(allGroupNames, cancellationToken);
                foreach (var existingGroup in existingGroups)
                {
                    if (!existingGroup.Realms.Any(r => r.RealmsName == realm))
                    {
                        existingGroup.Realms.Add(new GroupRealm
                        {
                            RealmsName = realm
                        });
                        _groupRepository.Update(existingGroup);
                    }
                }

                var existingGroupNames = existingGroups.Select(s => s.Name).ToList();
                var unknownGroups = groups.Where(s => !existingGroupNames.Contains(s.Name)).ToList();
                foreach (var unknownGroup in unknownGroups)
                {
                    unknownGroup.Realms = new List<GroupRealm>
                    {
                        new GroupRealm
                        {
                            RealmsName = realm
                        }
                    };
                }

                await _groupRepository.BulkAdd(unknownGroups);
            },
            (m, s, e, n) => m.MigrateGroups(s, e, n),
            cancellationToken);
    }

    public async Task<bool> MigrateUsers(string realm, string name, CancellationToken cancellationToken)
    {
        var migrationService = _migrationServiceFactory.Create(name);
        return await Migrate(
            realm,
            name,
            (m) => m.IsUsersMigrated,
            migrationService.NbUsers,
            async (e, c) =>
            {
                var users = await migrationService.ExtractUsers(e, c);
                var allUserIds = users.Select(s => s.Name).ToList();
                var existingUsers = await _userRepository.GetUsersBySubjects(allUserIds, cancellationToken);
                foreach (var existingUser in existingUsers)
                {
                    if (!existingUser.Realms.Any(r => r.RealmsName == realm))
                    {
                        existingUser.Realms.Add(new RealmUser
                        {
                            RealmsName = realm
                        });
                        _userRepository.Update(existingUser);
                    }
                }

                var existingUserNames = existingUsers.Select(s => s.Name).ToList();
                var unknownUsers = users.Where(s => !existingUserNames.Contains(s.Name)).ToList();
                foreach (var unknownUser in unknownUsers)
                {
                    unknownUser.Realms = new List<RealmUser>
                    {
                        new RealmUser
                        {
                            RealmsName = realm
                        }
                    };
                }

                await _userRepository.BulkAdd(unknownUsers);
            },
            (m, s, e, n) => m.MigrateUsers(s, e, n),
            cancellationToken);
    }

    private async Task MigrateScopes(List<Scope> scopes, Domains.Realm realm, CancellationToken cancellationToken)
    {
        var allScopeNames = scopes.Select(s => s.Name).ToList();
        var existingScopes = await _scopeRepository.GetByNames(allScopeNames, cancellationToken);
        foreach (var existingScope in existingScopes)
        {
            if (!existingScope.Realms.Any(r => r.Name == realm.Name))
            {
                existingScope.Realms.Add(realm);
                _scopeRepository.Update(existingScope);
            }
        }

        var existingScopeNames = existingScopes.Select(s => s.Name).ToList();
        var unknownScopes = scopes.Where(s => !existingScopeNames.Contains(s.Name)).ToList();
        foreach (var unknownScope in unknownScopes)
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

    private async Task<bool> HandleError(MigrationExecutionHistoryTypes type, string realm, string name, Func<string, string, CancellationToken, Task<bool>> cb, CancellationToken cancellationToken)
    {
        try
        {
            return await cb(realm, name, cancellationToken);
        }
        catch(Exception ex)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var migrationExecution = await _migrationStore.Get(realm, name, cancellationToken);
                migrationExecution.LogErrors(type, new List<string> { ex.ToString() });
                await transaction.Commit(cancellationToken);
                return false;
            }
        }
    }

    private MigrationOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(MigrationOptions).Name);
        return section.Get<MigrationOptions>() ?? new MigrationOptions();
    }
}
