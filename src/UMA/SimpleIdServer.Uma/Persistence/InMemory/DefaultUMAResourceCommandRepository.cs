using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAResourceCommandRepository : InMemoryCommandRepository<UMAResource>, IUMAResourceCommandRepository
    {
        public DefaultUMAResourceCommandRepository(List<UMAResource> umaResources) : base(umaResources)
        {
        }
    }
}
