// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Statistics;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.StatisticStore
{
    public class StatisticEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public StatisticEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage= sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(GetStatisticsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetStatsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var statisticResult = SidJsonSerializer.Deserialize<StatisticResult>(json);
            dispatcher.Dispatch(new GetStatisticsSuccessAction { NbClients = statisticResult.NbClients, NbUsers = statisticResult.NbUsers, NbInvalidAuthentications = statisticResult.InvalidAuthentications, NbValidAuthentications = statisticResult.ValidAuthentications });
        }

        private async Task<string> GetStatsUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/stats";
            }

            return $"{_options.IdServerBaseUrl}/stats";
        }
    }

    public class GetStatisticsAction
    {

    }

    public class GetStatisticsSuccessAction
    {
        public int NbUsers { get; set; }
        public int NbClients { get; set; }
        public int NbValidAuthentications { get; set; }
        public int NbInvalidAuthentications { get; set; }
    }
}
