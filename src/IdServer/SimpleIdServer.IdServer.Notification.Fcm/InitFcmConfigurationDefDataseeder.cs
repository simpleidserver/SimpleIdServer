﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Notification.Fcm;

public class InitFcmConfigurationDefDataseeder : BaseConfigurationDefinitionDataseeder<FcmOptions>
{
    public InitFcmConfigurationDefDataseeder(
        IConfigurationDefinitionStore configurationDefinitionStore,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(configurationDefinitionStore, transactionBuilder, dataSeederExecutionHistoryRepository)
    {
    }
    public override string Name => nameof(InitFcmConfigurationDefDataseeder);
}
