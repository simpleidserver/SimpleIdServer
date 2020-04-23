// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class InMemoryCommandRepository<T> : ICommandRepository<T> where T : ICloneable
    {
        private class DefaultTransaction : ITransaction
        {
            public Task Commit(CancellationToken token)
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {
            }
        }

        private readonly List<T> _lstData;

        public InMemoryCommandRepository(List<T> lstData)
        {
            _lstData = lstData;
        }

        public Task<ITransaction> StartTransaction(CancellationToken token)
        {
            ITransaction result = new DefaultTransaction();
            return Task.FromResult(result);
        }

        public Task<bool> Add(T data, CancellationToken token)
        {
            _lstData.Add((T)data.Clone());
            return Task.FromResult(true);
        }

        public Task<bool> Update(T data, CancellationToken token)
        {
            var record = _lstData.First(l => l.Equals(data));
            _lstData.Remove(record);
            _lstData.Add((T)record.Clone());
            return Task.FromResult(true);
        }

        public Task<bool> Delete(T data, CancellationToken token)
        {
            _lstData.Remove(_lstData.First(l => l.Equals(data)));
            return Task.FromResult(true);
        }
    }
}
