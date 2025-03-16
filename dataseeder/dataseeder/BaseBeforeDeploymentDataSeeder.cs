// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace DataSeeder;

public abstract class BaseBeforeDeploymentDataSeeder : BaseDataSeeder
{
    protected BaseBeforeDeploymentDataSeeder(IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
    }

    public override bool IsBeforeDeployment => true;
}
