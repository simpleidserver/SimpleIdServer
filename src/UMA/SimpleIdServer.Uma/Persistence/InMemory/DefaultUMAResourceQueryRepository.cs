using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAResourceQueryRepository : IUMAResourceQueryRepository
    {
        private List<UMAResource> _umaResources;

        public DefaultUMAResourceQueryRepository(List<UMAResource> umaResources)
        {
            _umaResources = umaResources;
        }

        public Task<UMAResource> FindByIdentifier(string id)
        {
            return Task.FromResult(_umaResources.FirstOrDefault(r => r.Id == id));
        }

        public Task<IEnumerable<UMAResource>> GetAll()
        {
            return Task.FromResult((IEnumerable<UMAResource>)_umaResources);
        }
    }
}
