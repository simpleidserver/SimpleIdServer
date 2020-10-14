using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Infrastructure.Lock
{
    public class InMemoryDistributedLock : IDistributedLock
    {
        private ICollection<string> _locks;

        public InMemoryDistributedLock()
        {
            _locks = new List<string>();
        }

        public async Task WaitLock(string id, CancellationToken token)
        {
            while(true)
            {
                Thread.Sleep(10);
                if (!await TryAcquireLock(id, token))
                {
                    continue;
                }

                return;
            }
        }

        public Task<bool> TryAcquireLock(string id, CancellationToken token)
        {
            lock(_locks)
            {
                if (_locks.Contains(id))
                {
                    return Task.FromResult(false);
                }

                _locks.Add(id);
                return Task.FromResult(true);
            }
        }

        public Task ReleaseLock(string id, CancellationToken token)
        {
            _locks.Remove(id);
            return Task.CompletedTask;
        }
    }
}
