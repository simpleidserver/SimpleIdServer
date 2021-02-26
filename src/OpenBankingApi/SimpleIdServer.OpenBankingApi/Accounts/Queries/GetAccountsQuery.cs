using MediatR;
using SimpleIdServer.OpenBankingApi.Accounts.Results;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries
{
    public class GetAccountsQuery : IRequest<GetAccountsResult>
    {
        public GetAccountsQuery(string accessToken, string issuer)
        {
            AccessToken = accessToken;
            Issuer = issuer;
        }

        public string AccessToken { get; set; }
        public string Issuer { get; set; }
    }
}
