// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ImportSummaryStore
{
    public class ImportSummaryEffects
    {
        private readonly IImportSummaryStore _importSummaryStore;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ImportSummaryEffects(IImportSummaryStore importSummaryStore, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _importSummaryStore = importSummaryStore;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }


        [EffectMethod]
        public async Task Handle(SearchImportSummariesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<ImportSummary> query = _importSummaryStore.Query().Where(c => c.RealmName == realm).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var nb = query.Count();
            var importSummaries = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchImportSummariesSuccessAction { ImportSummaries = importSummaries, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(LaunchImportAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            using (var httpClient = new HttpClient(handler))
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/import")
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var tt = "";
            }

            dispatcher.Dispatch(new LaunchImportSuccessAction());
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchImportSummariesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchImportSummariesSuccessAction
    {
        public IEnumerable<ImportSummary> ImportSummaries { get; set; } = new List<ImportSummary>();
        public int Count { get; set; }
    }

    public class LaunchImportAction
    {

    }

    public class LaunchImportSuccessAction
    {

    }
}
