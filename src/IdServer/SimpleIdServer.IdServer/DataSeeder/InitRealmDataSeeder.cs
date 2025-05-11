// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class InitRealmDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IRealmRepository _realmRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitRealmDataSeeder(
        IRealmRepository realmRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _realmRepository = realmRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitRealmDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var existingRealm = await _realmRepository.Get(DefaultRealms.Master.Name, cancellationToken);
            if(existingRealm == null)
            {
                _realmRepository.Add(DefaultRealms.Master);
                await transaction.Commit(cancellationToken);
            }
        }
    }
}
