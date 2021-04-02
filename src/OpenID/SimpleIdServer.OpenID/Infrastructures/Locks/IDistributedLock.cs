// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Infrastructures.Locks
{
    public interface IDistributedLock
    {
        Task<bool> TryAcquireLock(string id, CancellationToken token);
        Task ReleaseLock(string id, CancellationToken token);
    }
}
