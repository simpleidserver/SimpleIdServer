// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Light.Startup.Migrations.AfterDeployment;

public class InitDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;

    public InitDataSeeder(ITransactionBuilder transactionBuilder,
        IClientRepository clientRepository,
        IScopeRepository scopeRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
    }

    public override string Name => nameof(InitDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var scope = ScopeBuilder.CreateApiScope("api1", false).Build();
            var client = ClientBuilder.BuildApiClient("client", "secret").AddScope(scope).Build();
            _scopeRepository.Add(scope);
            _clientRepository.Add(client);
            await transaction.Commit(cancellationToken);
        }
    }
}
