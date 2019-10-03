using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
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

        public bool Update(T data)
        {
            var record = _lstData.First(l => l.Equals(data));
            _lstData.Remove(record);
            _lstData.Add((T)record.Clone());
            return true;
        }

        public bool Delete(T data)
        {
            _lstData.Remove(_lstData.First(l => l.Equals(data)));
            return true;
        }

        public Task<int> SaveChanges()
        {
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }
    }
}
