// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitAdministrativeScopeDataSeeder : BaseScopeDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitAdministrativeScopeDataSeeder(
        ITransactionBuilder transactionBuilder,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitAdministrativeScopeDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach(var scope in DefaultScopes.AdministrativeScopes)
            {
                await TryAddScope(scope, cancellationToken);
            }

            foreach(var scope in DefaultScopes.AdministrativeRoScopes)
            {
                await TryAddScope(scope, cancellationToken);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
