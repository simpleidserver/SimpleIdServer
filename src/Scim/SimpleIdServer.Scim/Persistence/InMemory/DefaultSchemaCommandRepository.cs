using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSchemaCommandRepository : InMemoryCommandRepository<SCIMSchema>, ISCIMSchemaCommandRepository
    {
        public DefaultSchemaCommandRepository(List<SCIMSchema> lstData) : base(lstData)
        {
        }
    }
}
