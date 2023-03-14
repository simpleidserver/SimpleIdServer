// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.Auditing
{
    public class AuditingRecordReducer
    {
        #region SearchAuditingRecordsState

        [ReducerMethod]
        public static SearchAuditingRecordsState ReduceSearchAuditingRecordsAction(SearchAuditingRecordsState state, SearchAuditingRecordsAction act) => new(isLoading: true, auditEvents: new List<AuditEvent>());

        [ReducerMethod]
        public static SearchAuditingRecordsState ReduceSearchAuditingRecordsSuccessAction(SearchAuditingRecordsState state, SearchAuditingRecordsSuccessAction act)
        {
            var auditEvents = act.AuditEvents.ToList();
            return state with
            {
                IsLoading = false,
                AuditEvents = auditEvents,
                Count = act.Count
            };
        }

        #endregion
    }
}
