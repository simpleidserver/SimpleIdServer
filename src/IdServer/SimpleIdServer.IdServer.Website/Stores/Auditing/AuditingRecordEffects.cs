// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.Auditing
{
    public class AuditingRecordEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IRealmStore _realmStore;
        private readonly IdServerWebsiteOptions _options;

        public AuditingRecordEffects(
            IWebsiteHttpClientFactory websiteHttpClientFactory, 
            IOptions<IdServerWebsiteOptions> options,
            IRealmStore realmStore)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _realmStore = realmStore;
        }

        [EffectMethod]
        public async Task Handle(SearchAuditingRecordsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/.search"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new SearchAuditingRequest
                {
                    Filter = SanitizeExpression(action.Filter),
                    OrderBy = SanitizeExpression(action.OrderBy),
                    Skip = action.Skip,
                    Take = action.Take,
                    DisplayOnlyErrors = action.DisplayOnlyErrors
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<AuditEvent>>(json);
            dispatcher.Dispatch(new SearchAuditingRecordsSuccessAction { AuditEvents = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        private async Task<string> GetBaseUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = _realmStore.Realm;
                var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.Issuer}/{realmStr}/auditing";
            }

            return $"{_options.Issuer}/auditing";
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
