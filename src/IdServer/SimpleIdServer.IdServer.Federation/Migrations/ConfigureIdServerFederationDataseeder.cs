// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Federation.Migrations;

public class ConfigureIdServerFederationDataseeder : BaseScopeDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public ConfigureIdServerFederationDataseeder(ITransactionBuilder transctionBuilder, IRealmRepository realmRepository, IScopeRepository scopeRepository, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transctionBuilder;
    }

    public override string Name => nameof(ConfigureIdServerFederationDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            await TryAddScope(FederationEntitiesScope, cancellationToken);
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
