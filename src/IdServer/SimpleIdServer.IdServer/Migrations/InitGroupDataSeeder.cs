// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitGroupDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IRealmRepository _realmRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitGroupDataSeeder(
        IRealmRepository realmRepository,
        IGroupRepository groupRepository,
        IScopeRepository scopeRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _realmRepository = realmRepository;
        _groupRepository = groupRepository;
        _scopeRepository = scopeRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitGroupDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transactionBuilder = _transactionBuilder.Build())
        {
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            foreach(var group in DefaultGroups.All)
            {
                var existingGroup = await _groupRepository.Get(Constants.DefaultRealm, group.Id, cancellationToken);
                if (existingGroup != null)
                {
                    continue;
                }

                var roleNames = group.Roles.Select(r => r.Name).ToList();
                var existingScopes = await _scopeRepository.GetByNames(Constants.DefaultRealm, roleNames, cancellationToken);
                group.Roles = existingScopes;
                _groupRepository.Add(group);
            }

            await transactionBuilder.Commit(cancellationToken);
        }
    }
}
