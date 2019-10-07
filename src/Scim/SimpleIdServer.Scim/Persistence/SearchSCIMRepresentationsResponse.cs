using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence
{
    public class SearchSCIMRepresentationsResponse
    {
        public SearchSCIMRepresentationsResponse(int totalResult, IEnumerable<SCIMRepresentation> content)
        {
            TotalResults = totalResult;
            Content = content;
        }

        public int TotalResults { get; set; }
        public IEnumerable<SCIMRepresentation> Content { get; set; }
    }
}
