using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAResourceQueryRepository
    {
        Task<IEnumerable<UMAResource>> GetAll();
        Task<UMAResource> FindByIdentifier(string id);
    }
}
