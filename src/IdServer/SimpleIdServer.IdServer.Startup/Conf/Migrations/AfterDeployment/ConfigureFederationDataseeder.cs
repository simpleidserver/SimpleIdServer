// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Federation.Migrations;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureFederationDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;

    public ConfigureFederationDataseeder(ITransactionBuilder transctionBuilder, IClientRepository clientRepository, IScopeRepository scopeRepository, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transctionBuilder;
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
    }

    public override string Name => nameof(ConfigureFederationDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var existingScope = await _scopeRepository.GetByName(Constants.DefaultRealm, ConfigureIdServerFederationDataseeder.FederationEntitiesScope.Name, cancellationToken);
            var existingClient = await _clientRepository.GetByClientId(Constants.DefaultRealm, DefaultClients.SidAdminClientId, cancellationToken);
            if (existingClient != null && existingScope != null)
            {
                existingClient.Scopes.Add(existingScope);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
