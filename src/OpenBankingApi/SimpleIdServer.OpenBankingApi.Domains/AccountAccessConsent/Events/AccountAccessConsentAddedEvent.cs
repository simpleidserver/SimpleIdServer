using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events
{
    [DebuggerDisplay("Account Access Consent has been added")]
    [Serializable]
    public class AccountAccessConsentAddedEvent : DomainEvent
    {
        public AccountAccessConsentAddedEvent(string id, string aggregateId, int version, ICollection<string> permissions, DateTime? expirationDateTime, DateTime? transactionFromDateTime, DateTime? transactionToDateTime) : base(id, aggregateId, version)
        {
            Permissions = permissions;
            TransactionFromDateTime = transactionFromDateTime;
            TransactionToDateTime = transactionToDateTime;
        }

        public ICollection<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? TransactionFromDateTime { get; set; }
        public DateTime? TransactionToDateTime { get; set; }
    }
}
