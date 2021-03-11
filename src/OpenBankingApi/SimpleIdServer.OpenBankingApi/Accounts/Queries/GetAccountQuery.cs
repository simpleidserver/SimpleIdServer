using MediatR;
using SimpleIdServer.OpenBankingApi.Accounts.Results;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries
{
    public class GetAccountQuery : IRequest<GetAccountsResult>
    {
        public GetAccountQuery(string token, string accountId, string issuer)
        {
            Token = token;
            AccountId = accountId;
            Issuer = issuer;
        }

        public string Token { get; set; }
        public string AccountId { get; set; }
        public string Issuer { get; set; }
    }
}
