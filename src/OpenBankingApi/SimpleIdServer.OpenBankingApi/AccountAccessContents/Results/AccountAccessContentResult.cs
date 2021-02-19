using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Results
{
    public class AccountAccessContentResult
    {
        public AddAccountAccessContentData Data { get; set; }

        public static AccountAccessContentResult ToDto(AccountAccessConsentAggregate accountAccessConsent)
        {
            return new AccountAccessContentResult
            {
                Data = new AddAccountAccessContentData
                {
                    ExpirationDateTime = accountAccessConsent.ExpirationDateTime,
                    Permissions = accountAccessConsent.Permissions.Select(p => p.Name).ToList(),
                    TransactionFromDateTime = accountAccessConsent.TransactionFromDateTime,
                    TransactionToDateTime = accountAccessConsent.TransactionToDateTime
                }
            };
        }
    }

    public class AddAccountAccessContentData
    {
        public ICollection<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? TransactionFromDateTime { get; set; }
        public DateTime? TransactionToDateTime { get; set; }
    }
}
