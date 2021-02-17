using MediatR;
using SimpleIdServer.OpenBankingApi.Accounts.Results;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries
{
    public class GetAccountQuery : IRequest<AccountResult>
    {
        public GetAccountQuery(string accountId)
        {
            AccountId = accountId;
        }

        public string AccountId { get; set; }
    }
}
