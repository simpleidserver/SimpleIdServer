using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMSchemaQueryRepository
    {
        Task<SCIMSchema> FindSCIMSchemaById(string schemaId);
        Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers);
        Task<IEnumerable<SCIMSchema>> GetAll();
    }
}
