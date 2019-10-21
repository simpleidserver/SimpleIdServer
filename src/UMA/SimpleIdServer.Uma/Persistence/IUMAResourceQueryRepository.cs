using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAResourceQueryRepository
    {
        Task<IEnumerable<UMAResource>> GetAll();
        Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchUMAResourceParameter);
        Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids);
        Task<UMAResource> FindByIdentifier(string id);
    }
}
