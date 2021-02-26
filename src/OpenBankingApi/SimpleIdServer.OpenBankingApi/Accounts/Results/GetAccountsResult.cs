using SimpleIdServer.OpenBankingApi.Common.Results;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Accounts.Results
{
    public class GetAccountsResult : BaseResult
    {
        public GetAccountsResult(string self, int totalPages) : base(self, totalPages)
        {
        }

        public AccountsResult Data { get; set; }

        public static GetAccountsResult ToResult(IEnumerable<AccountAggregate> accounts, IEnumerable<AccountAccessConsentPermission> permissions, string self, int totalPages)
        {
            return new GetAccountsResult(self, totalPages)
            {
                Data = new AccountsResult
                {
                    Account = accounts.Select(a => AccountRecordResult.ToResult(a, permissions))
                }
            };
        }
    }
}
