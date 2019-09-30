using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleIdServer.Scim.Domain;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSchemaQueryRepository : ISCIMSchemaQueryRepository
    {
        private readonly List<SCIMSchema> _schemas;

        public DefaultSchemaQueryRepository(List<SCIMSchema> schemas)
        {
            _schemas = schemas;
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            return Task.FromResult(_schemas.FirstOrDefault(s => s.Id == schemaId));
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            return Task.FromResult(_schemas.Where(s => schemaIdentifiers.Contains(s.Id)));
        }
    }
}
