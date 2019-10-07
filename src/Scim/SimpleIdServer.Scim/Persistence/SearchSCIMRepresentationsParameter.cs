using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;

namespace SimpleIdServer.Scim.Persistence
{
    public class SearchSCIMRepresentationsParameter
    {
        public SearchSCIMRepresentationsParameter(int startIndex, int count, string sortBy, SearchSCIMRepresentationOrders? sortOrder = null, SCIMExpression filter = null)
        {
            StartIndex = startIndex;
            Count = count;
            SortBy = sortBy;
            SortOrder = sortOrder;
            Filter = filter;
        }

        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string SortBy { get; set; }
        public SearchSCIMRepresentationOrders? SortOrder { get; set; }
        public SCIMExpression Filter { get; set; } 
    }
}
