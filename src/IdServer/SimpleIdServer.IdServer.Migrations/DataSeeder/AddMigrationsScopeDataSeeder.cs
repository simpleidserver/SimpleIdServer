// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Migrations.DataSeeder;

public class AddMigrationsScopeDataSeeder : BaseScopeDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;

    public AddMigrationsScopeDataSeeder(
        ITransactionBuilder transactionBuilder,
        IClientRepository clientRepository,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _clientRepository = clientRepository;
    }

    public override string Name => nameof(AddMigrationsScopeDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var client = await _clientRepository.GetByClientId(IdServer.Constants.DefaultRealm, DefaultClients.SidAdminClientId, cancellationToken);
            if(client == null)
            {
                return;
            }

            var migrationsScope = await TryAddScope(DefaultScopes.Migrations, cancellationToken);
            var existingScope = client.Scopes.FirstOrDefault(s => s.Name == DefaultScopes.Migrations.Name);
            if (existingScope != null)
            {
                client.Scopes.Add(existingScope);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
