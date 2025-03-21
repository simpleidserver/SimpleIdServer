// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public abstract class BaseScopeDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;

    public BaseScopeDataseeder(
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
    }

    protected async Task<Scope> TryAddScope(Scope scope, CancellationToken cancellationToken)
    {
        var existingScope = await _scopeRepository.GetByName(Constants.DefaultRealm, scope.Name, cancellationToken);
        if (existingScope != null)
        {
            return existingScope;
        }

        var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
        scope.Realms = new List<Realm>
        {
            masterRealm
        };
        _scopeRepository.Add(scope);
        return scope;
    }
}
