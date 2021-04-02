// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OpenID.Infrastructures.Locks;
using SimpleIdServer.OpenID.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Infrastructures.Jobs
{
    public abstract class BaseScheduledJob : IJob
    {
        public BaseScheduledJob(
            IOptions<OpenIDHostOptions> options,
            ILogger<BaseScheduledJob> logger,
            IDistributedLock distributedLock)
        {
            Options = options.Value;
            Logger = logger;
            DistributedLock = distributedLock;
        }

        protected OpenIDHostOptions Options { get; private set; }
        protected ILogger<BaseScheduledJob> Logger { get; private set; }
        protected IDistributedLock DistributedLock { get; private set; }
        protected CancellationTokenSource CancellationTokenSource { get; private set; }
        protected Task CurrentTask { get; private set; }
        protected DateTime? NextExecutionDateTime { get; private set; }
        protected abstract string LockName { get; }

        public Task Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CurrentTask = new Task(Handle, TaskCreationOptions.LongRunning);
            CurrentTask.Start();
            return Task.CompletedTask;
        }

        public Task Cancel()
        {
            return Task.CompletedTask;
        }

        private async void Handle()
        {
            var cancellationToken = CancellationTokenSource.Token;
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(Options.BlockThreadMS);
                if (NextExecutionDateTime != null && DateTime.UtcNow <= NextExecutionDateTime.Value)
                {
                    continue;
                }

                if (!await DistributedLock.TryAcquireLock(LockName, cancellationToken))
                {
                    continue;
                }

                try
                {
                    await Execute(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
                finally
                {
                    await DistributedLock.ReleaseLock(LockName, cancellationToken);
                    NextExecutionDateTime = DateTime.UtcNow.AddMilliseconds(Options.BlockThreadMS);
                }
            }
        }

        protected abstract Task Execute(CancellationToken cancellationToken);
    }
}
