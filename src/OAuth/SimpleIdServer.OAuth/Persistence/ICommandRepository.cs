// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface ICommandRepository<T> : IDisposable
    {
        Task<bool> Add(T data, CancellationToken token);
        Task<bool> Update(T data, CancellationToken token);
        Task<bool> Delete(T data, CancellationToken token);
        Task<int> SaveChanges(CancellationToken token);
    }
}