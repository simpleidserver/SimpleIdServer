// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.Auditing
{
    [FeatureState]
    public record SearchAuditingRecordsState
    {
        public SearchAuditingRecordsState() { }

        public SearchAuditingRecordsState(bool isLoading, IEnumerable<AuditEvent> auditEvents)
        {
            AuditEvents = auditEvents;
            Count = auditEvents.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<AuditEvent>? AuditEvents { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }
}
