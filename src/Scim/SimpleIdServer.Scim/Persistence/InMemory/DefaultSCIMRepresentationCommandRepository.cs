using System.Collections.Generic;
using SimpleIdServer.Scim.Domain;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationCommandRepository : InMemoryCommandRepository<SCIMRepresentation>, ISCIMRepresentationCommandRepository
    {
        public DefaultSCIMRepresentationCommandRepository(List<SCIMRepresentation> lstData) : base(lstData)
        {
        }
    }
}
