using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Infrastructure.Lock
{
    public interface IDistributedLock
    {
        Task WaitLock(string id, CancellationToken token);
        Task<bool> TryAcquireLock(string id, CancellationToken token);
        Task ReleaseLock(string id, CancellationToken token);
    }
}
