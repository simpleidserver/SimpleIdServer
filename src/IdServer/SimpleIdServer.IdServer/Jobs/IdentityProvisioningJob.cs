// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public interface IIdentityProvisioningJob
    {
        public string Name { get; }
        public Task Execute(string instanceId, CancellationToken cancellationToken);
    }

    public abstract class IdentityProvisioningJob : IIdentityProvisioningJob
    {
        public abstract string Name { get; }
        public abstract Task Execute(string instanceId, CancellationToken cancellationToken);
    }
}
