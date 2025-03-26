// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureFastfedDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IUserRepository _userRepository;

    public ConfigureFastfedDataSeeder(ITransactionBuilder transactionBuilder,
        IClientRepository clientRepository,
        IGroupRepository groupRepository,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IUserRepository userRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _clientRepository = clientRepository;
        _groupRepository = groupRepository;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _userRepository = userRepository;
    }

    public override string Name => nameof(ConfigureFastfedDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            var adminUser = await _userRepository.GetBySubject(DefaultUsers.Administrator.Name, Constants.DefaultRealm, cancellationToken);
            var scopeNames = AllClients.SelectMany(c => c.Scopes.Select(s => s.Name)).Distinct().ToList();
            var clientIds = AllClients.Select(c => c.ClientId).ToList();
            var existingScopes = await _scopeRepository.GetByNames(Constants.DefaultRealm, scopeNames, cancellationToken);
            var existingGroups = await _groupRepository.GetAllByStrictFullPath(Constants.DefaultRealm, AllGroups.Select(g => g.FullPath).ToList(), cancellationToken);
            var existingClients = await _clientRepository.GetByClientIds(Constants.DefaultRealm, clientIds, cancellationToken);
            foreach (var scope in AllScopes)
            {
                var existingScope = existingScopes.SingleOrDefault(s => s.Name == scope.Name);
                if (existingScope == null)
                {
                    scope.Realms = new List<Realm>
                    {
                        masterRealm
                    };
                    existingScopes.Add(scope);
                    _scopeRepository.Add(scope);
                }
            }

            foreach (var group in AllGroups)
            {
                var existingGroup = existingGroups.SingleOrDefault(g => g.FullPath == group.FullPath);
                if (existingGroup == null)
                {
                    group.Roles = existingScopes.Where(s => group.Roles.Any(r => r.Name == s.Name)).ToList();
                    existingGroups.Add(group);
                    _groupRepository.Add(group);
                }
            }

            foreach (var client in AllClients)
            {
                var existingClient = existingClients.SingleOrDefault(c => c.ClientId == client.ClientId);
                if (existingClient != null)
                {
                    continue;
                }

                client.Realms = new List<Realm>
                {
                    masterRealm
                };
                client.Scopes = existingScopes.Where(s => client.Scopes.Any(cs => cs.Name == s.Name)).ToList();
                _clientRepository.Add(client);
            }

            if (!adminUser.Groups.Any(g => g.Group.Name == FastFedAdministratorGroup.Name))
            {
                adminUser.Groups.Add(new GroupUser
                {
                    GroupsId = FastFedAdministratorGroup.Id
                });
            }

            await transaction.Commit(cancellationToken);
        }
    }

    private static List<Client> AllClients => new List<Client>
    {
        IdentityProviderClient,
        ApplicationProviderClient,
        FastFedClient,
        ScimClient
    };

    private static List<Group> AllGroups => new List<Group>
    {
        FastFedAdministratorGroup
    };

    private static List<Scope> AllScopes => new List<Scope>
    {
        IdProviderAdministratorScope,
        AppProviderAdministratorScope,
        ScimScope
    };

    private static Client IdentityProviderClient => ClientBuilder.BuildTraditionalWebsiteClient("identityProvider", "password", null, "https://localhost:5020/*").EnableClientGrantType()
            .SetClientName("Identity Provider")
            .AddScope(
                DefaultScopes.Role,
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                DefaultScopes.Groups).Build();

    private static Client ApplicationProviderClient => ClientBuilder.BuildTraditionalWebsiteClient("applicationProvider", "password", null, "https://localhost:5021/*").EnableClientGrantType()
            .SetClientName("Application Provider")
            .AddScope(
                DefaultScopes.Role,
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                DefaultScopes.Groups).Build();

    private static Client FastFedClient => ClientBuilder.BuildApiClient("fastFed", "password")
            .AddScope(
                DefaultScopes.Clients,
                DefaultScopes.Scopes
            ).Build();

    private static Client ScimClient => ClientBuilder.BuildApiClient("scimClient", "password")
            .AddScope(ScimScope)
            .Build();

    private static Scope IdProviderAdministratorScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "identityProvider/administrator",
        Type = ScopeTypes.ROLE,
        Protocol = ScopeProtocols.OAUTH,
        Description = "Administrator",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow,
    };

    private static Scope AppProviderAdministratorScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "applicationProvider/administrator",
        Type = ScopeTypes.ROLE,
        Protocol = ScopeProtocols.OAUTH,
        Description = "Administrator",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow,
    }; 
    
    private static Scope ScimScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "scim",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.APIRESOURCE,
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };

    private static Group FastFedAdministratorGroup = new Group
    {
        Id = "fc21e468-7db3-4882-8228-63a7622ee58d",
        CreateDateTime = DateTime.UtcNow,
        FullPath = "fastFedAdministrator",
        Realms = new List<GroupRealm>
        {
            new GroupRealm
            {
                RealmsName = DefaultRealms.Master.Name
            }
        },
        Name = "fastFedAdministrator",
        Description = "Administrator role for fastfed",
        Roles = new List<Scope>
        {
            IdProviderAdministratorScope,
            AppProviderAdministratorScope
        }
    };
}
