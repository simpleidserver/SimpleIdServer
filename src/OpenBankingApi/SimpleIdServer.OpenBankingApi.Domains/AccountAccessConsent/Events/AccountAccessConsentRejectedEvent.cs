using System;
using System.Diagnostics;

namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events
{
    [DebuggerDisplay("Account Access Consent is rejected")]
    public class AccountAccessConsentRejectedEvent : DomainEvent
    {
        public AccountAccessConsentRejectedEvent(string id, string aggregateId, int version, DateTime updateDateTime) : base(id, aggregateId, version)
        {
            UpdateDateTime = updateDateTime;
        }

        public DateTime UpdateDateTime { get; set; }
    }
}
