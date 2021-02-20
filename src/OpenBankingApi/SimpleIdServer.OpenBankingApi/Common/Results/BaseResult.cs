namespace SimpleIdServer.OpenBankingApi.Common.Results
{
    public class BaseResult
    {
        public BaseResult(string self, int totalPages)
        {
            Links = new LinksResult
            {
                Self = self
            };
            Meta = new MetaResult
            {
                TotalPages = totalPages
            };
        }

        public LinksResult Links { get; set; }
        public MetaResult Meta { get; set; }
    }
}
