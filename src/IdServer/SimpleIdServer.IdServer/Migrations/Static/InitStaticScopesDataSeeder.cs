// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations.Static;

public class InitStaticScopesDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IScopeRepository _scopeRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly StaticScopesDataSeeder _staticScopes;

    public InitStaticScopesDataSeeder(
        IScopeRepository scopeRepository,
        ITransactionBuilder transactionBuilder,
        StaticScopesDataSeeder staticScopes,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _scopeRepository = scopeRepository;
        _transactionBuilder = transactionBuilder;
        _staticScopes = staticScopes;
    }

    public override string Name => nameof(InitStaticScopesDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach (var scope in _staticScopes.Scopes)
            {
                _scopeRepository.Add(scope);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}

public class StaticScopesDataSeeder
{
    public StaticScopesDataSeeder(List<Scope> scopes)
    {
        Scopes = scopes;
    }

    public List<Scope> Scopes
    {
        get; private set;
    }
}