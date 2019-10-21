using System.Collections.Generic;

namespace SimpleIdServer.Uma.Persistence
{
    public class SearchResult<T>
    {
        public int TotalResults { get; set; }
        public ICollection<T> Records { get; set; }
    }
}
