// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Hangfire;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs;

public abstract class BaseJob : IJob
{
    private readonly IRecurringJobStatusRepository _recurringJobStatusRepository;

    protected BaseJob(IRecurringJobStatusRepository recurringJobStatusRepository)
    {
        _recurringJobStatusRepository = recurringJobStatusRepository;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    public async Task Execute()
    {
        var result = await _recurringJobStatusRepository.Get(this.GetType().Name, CancellationToken.None);
        if(result != null && result.IsDisabled)
        {
            return;
        }

        await ExecuteInternal();
    }

    protected abstract Task ExecuteInternal();
}
