// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitScopeDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitScopeDataSeeder(
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitScopeDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var names = DefaultScopes.All.Select(s => s.Name).ToList();
            var existingScopes = await _scopeRepository.GetByNames(Constants.DefaultRealm, names, cancellationToken);
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            var unknownScopes = DefaultScopes.All.Where(s => !existingScopes.Any(es => es.Name == s.Name)).ToList();
            foreach(var unknownScope in unknownScopes)
            {
                unknownScope.Realms = new List<Realm>
                {
                    masterRealm
                };
                _scopeRepository.Add(unknownScope);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
