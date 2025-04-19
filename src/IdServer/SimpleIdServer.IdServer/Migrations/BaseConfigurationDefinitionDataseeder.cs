// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public abstract class BaseConfigurationDefinitionDataseeder<T> : BaseAfterDeploymentDataSeeder
{
    private readonly IConfigurationDefinitionStore _configurationDefinitionStore;
    private readonly ITransactionBuilder _transactionBuilder;

    protected BaseConfigurationDefinitionDataseeder(
        IConfigurationDefinitionStore configurationDefinitionStore,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _configurationDefinitionStore = configurationDefinitionStore;
        _transactionBuilder = transactionBuilder;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        var configurationDefinition = ConfigurationDefinitionExtractor.Extract(typeof(T));
        var existingConfigurationDefinition = await _configurationDefinitionStore.Get(configurationDefinition.Id, cancellationToken);
        if (existingConfigurationDefinition != null)
        {
            return;
        }

        using(var transaction = _transactionBuilder.Build())
        {
            _configurationDefinitionStore.Add(configurationDefinition);
            await transaction.Commit(cancellationToken);
        }
    }
}
