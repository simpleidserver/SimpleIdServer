using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Persistence.Results
{
    public class SearchResult<T>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public int TotalLength { get; set; }
        public ICollection<T> Content { get; set; }
    }
}