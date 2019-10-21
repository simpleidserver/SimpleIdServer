using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAPendingRequestQueryRepository
    {
        Task<SearchResult<UMAPendingRequest>> FindByOwner(string owner, SearchRequestParameter searchRequestParameter);
        Task<SearchResult<UMAPendingRequest>> FindByRequester(string requester, SearchRequestParameter searchRequestParameter);
        Task<IEnumerable<UMAPendingRequest>> FindByTicketIdentifier(string ticketIdentifier);
        Task<UMAPendingRequest> FindByTicketIdentifierAndOwner(string ticketIdentifier, string owner);
    }
}
