// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class InMemoryCommandRepository<T> : ICommandRepository<T> where T : ICloneable
    {
        private readonly List<T> _lstData;

        public InMemoryCommandRepository(List<T> lstData)
        {
            _lstData = lstData;
        }

        public bool Add(T data)
        {
            _lstData.Add((T)data.Clone());
            return true;
        }

        public Task<bool> Update(T data, CancellationToken token)
        {
            var record = _lstData.First(l => l.Equals(data));
            _lstData.Remove(record);
            _lstData.Add((T)record.Clone());
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public bool Delete(T data)
        {
            var record = _lstData.First(l => l.Equals(data));
            _lstData.Remove(record);
            return true;
        }

        public void Dispose()
        {
        }
    }
}
