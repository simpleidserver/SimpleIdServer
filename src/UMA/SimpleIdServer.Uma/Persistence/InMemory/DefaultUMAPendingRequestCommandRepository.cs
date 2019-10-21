using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAPendingRequestCommandRepository : InMemoryCommandRepository<UMAPendingRequest>, IUMAPendingRequestCommandRepository
    {
        public DefaultUMAPendingRequestCommandRepository(List<UMAPendingRequest> umaPendingRequests) : base(umaPendingRequests)
        {
        }
    }
}
