// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.Auditing
{
    public class AuditingRecordEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AuditingRecordEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage protectedSessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = protectedSessionStorage;
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
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/auditing";
            }

            return $"{_options.IdServerBaseUrl}/auditing";
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
