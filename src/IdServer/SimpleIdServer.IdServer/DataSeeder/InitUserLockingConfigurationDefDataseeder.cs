// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.DataSeeder;

public class InitUserLockingConfigurationDefDataseeder : BaseConfigurationDefinitionDataseeder<UserLockingOptions>
{
    public InitUserLockingConfigurationDefDataseeder(
        IConfigurationDefinitionStore configurationDefinitionStore,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(configurationDefinitionStore, transactionBuilder, dataSeederExecutionHistoryRepository)
    {
    }
    public override string Name => nameof(InitUserLockingConfigurationDefDataseeder);
}