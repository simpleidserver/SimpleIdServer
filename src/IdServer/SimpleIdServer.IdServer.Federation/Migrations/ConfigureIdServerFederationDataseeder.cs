// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Federation.Migrations;

public class ConfigureIdServerFederationDataseeder : BaseScopeDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;

    public ConfigureIdServerFederationDataseeder(ITransactionBuilder transctionBuilder, IClientRepository clientRepository, IRealmRepository realmRepository, IScopeRepository scopeRepository, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transctionBuilder;
        _clientRepository = clientRepository;
    }

    public override string Name => nameof(ConfigureIdServerFederationDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var existingScope = await TryAddScope(FederationEntitiesScope, cancellationToken);
            var existingClient = await _clientRepository.GetByClientId(Constants.DefaultRealm, DefaultClients.SidAdminClientId, cancellationToken);
            if (existingClient != null)
            {
                existingClient.Scopes.Add(existingScope);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    public static Scope FederationEntitiesScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "federation_entities",
        Realms = new List<Realm>
            {
                Config.DefaultRealms.Master
            },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
}
