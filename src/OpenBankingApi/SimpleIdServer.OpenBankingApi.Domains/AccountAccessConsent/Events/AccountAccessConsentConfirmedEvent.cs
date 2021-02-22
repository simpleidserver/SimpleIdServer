// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informati

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events
{
    [DebuggerDisplay("Account Access Consent is confirmed")]
    public class AccountAccessConsentConfirmedEvent : DomainEvent
    {
        public AccountAccessConsentConfirmedEvent(string id, string aggregateId, int version, IEnumerable<string> accountIds, DateTime updateDateTime) : base(id, aggregateId, version)
        {
            AccountIds = accountIds;
            UpdateDateTime = updateDateTime;
        }

        public IEnumerable<string> AccountIds { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
