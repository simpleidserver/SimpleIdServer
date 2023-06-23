// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.Auditing
{
    public class AuditingRecordEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AuditingRecordEffects(IDbContextFactory<StoreDbContext> factory, ProtectedSessionStorage protectedSessionStorage)
        {
            _factory = factory;
            _sessionStorage = protectedSessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchAuditingRecordsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                IQueryable<AuditEvent> query = dbContext.AuditEvents.AsNoTracking().Where(r => r.Realm == realm);
                if (action.DisplayOnlyErrors)
                    query = query.Where(r => r.IsError);

                if (!string.IsNullOrWhiteSpace(action.Filter))
                    query = query.Where(SanitizeExpression(action.Filter));

                if (!string.IsNullOrWhiteSpace(action.OrderBy))
                    query = query.OrderBy(SanitizeExpression(action.OrderBy));

                var nb = query.Count();
                var auditEvents = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
                dispatcher.Dispatch(new SearchAuditingRecordsSuccessAction { AuditEvents = auditEvents, Count = nb });
            }

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchAuditingRecordsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public bool DisplayOnlyErrors { get; set; }
    }

    public class SearchAuditingRecordsSuccessAction
    {
        public IEnumerable<AuditEvent> AuditEvents { get; set; } = new List<AuditEvent>();
        public int Count { get; set; }
    }
}
