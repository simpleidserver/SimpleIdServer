namespace SimpleIdServer.Uma.Persistence
{
    public class SearchRequestParameter
    {
        public SearchRequestParameter()
        {
            StartIndex = 0;
            Count = 100;
            SortKey = "CreateDateTime";
            SortOrder = SearchRequestOrders.DESC;
        }

        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string SortKey { get; set; }
        public SearchRequestOrders SortOrder { get; set; }
    }
}
