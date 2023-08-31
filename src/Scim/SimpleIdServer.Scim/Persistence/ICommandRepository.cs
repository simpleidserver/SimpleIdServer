// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ICommandRepository<T>
    {
        Task<T> Get(string id, bool include = false, CancellationToken token = default(CancellationToken));
        Task<ITransaction> StartTransaction(CancellationToken token = default(CancellationToken));
        Task<bool> Add(T data, CancellationToken token = default(CancellationToken));
        Task<bool> Update(T data, CancellationToken token = default(CancellationToken));
        Task<bool> Delete(T data, CancellationToken token = default(CancellationToken));
    }
}
