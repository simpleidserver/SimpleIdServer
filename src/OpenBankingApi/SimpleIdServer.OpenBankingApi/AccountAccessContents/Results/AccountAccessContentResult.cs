using Newtonsoft.Json.Linq;
using SimpleIdServer.OpenBankingApi.Common.Results;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Results
{
    public class AccountAccessContentResult : BaseResult
    {
        private AccountAccessContentResult(string self, int totalPages) : base(self, totalPages) { }

        public AddAccountAccessContentData Data { get; set; }
        public JObject Risk { get; set; }

        public static AccountAccessContentResult ToDto(AccountAccessConsentAggregate accountAccessConsent, string self, int totalPages)
        {
            JObject risk = string.IsNullOrEmpty(accountAccessConsent.Risk) ? null : JObject.Parse(accountAccessConsent.Risk);
            return new AccountAccessContentResult(self, totalPages)
            {
                Data = new AddAccountAccessContentData
                {
                    ConsentId = accountAccessConsent.AggregateId,
                    Status = accountAccessConsent.Status.Name,
                    ExpirationDateTime = accountAccessConsent.ExpirationDateTime,
                    Permissions = accountAccessConsent.Permissions.Select(p => p.Name).ToList(),
                    TransactionFromDateTime = accountAccessConsent.TransactionFromDateTime,
                    TransactionToDateTime = accountAccessConsent.TransactionToDateTime
                },
                Risk = risk
            };
        }
    }

    public class AddAccountAccessContentData
    {
        public string ConsentId { get; set; }
        public string Status { get; set; }
        public ICollection<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? TransactionFromDateTime { get; set; }
        public DateTime? TransactionToDateTime { get; set; }
    }
}
