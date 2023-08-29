// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class InMemoryCommandRepository<T> : ICommandRepository<T> where T : BaseDomain
    {
        private readonly List<T> _lstData;

        public InMemoryCommandRepository(List<T> lstData)
        {
            _lstData = lstData;
        }

        protected List<T> LstData { get => _lstData; }

        public Task<T> Get(string id, CancellationToken token)
        {
            return Task.FromResult(_lstData.FirstOrDefault(r => r.Id == id));
        }

        public Task<ITransaction> StartTransaction(CancellationToken token)
        {
            ITransaction result = new DefaultTransaction();
            return Task.FromResult(result);
        }

        public virtual Task<bool> Add(T data, CancellationToken token)
        {
            _lstData.Add((T)data.Clone());
            return Task.FromResult(true);
        }

        public virtual Task<bool> Update(T data, CancellationToken token)
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
