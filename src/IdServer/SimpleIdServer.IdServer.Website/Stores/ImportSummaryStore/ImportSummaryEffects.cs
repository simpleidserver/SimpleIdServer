// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.ImportSummaryStore
{
    public class ImportSummaryEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ImportSummaryEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }


        [EffectMethod]
        public async Task Handle(SearchImportSummariesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/import/.search"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new SearchRequest
                {
                    Filter = SanitizeExpression(action.Filter),
                    OrderBy = SanitizeExpression(action.OrderBy),
                    Skip = action.Skip,
                    Take = action.Take
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<SearchResult<Domains.ImportSummary>>(json);
            dispatcher.Dispatch(new SearchImportSummariesSuccessAction { ImportSummaries = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(LaunchImportAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/import")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new LaunchImportSuccessAction());
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = await GetRealm();
                return $"{_options.IdServerBaseUrl}/{realm}/provisioning";
            }

            return $"{_options.IdServerBaseUrl}/provisioning";
        }

        private async Task<string> GetRealm()
        {
            if (!_options.IsReamEnabled) return SimpleIdServer.IdServer.Constants.DefaultRealm;
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
