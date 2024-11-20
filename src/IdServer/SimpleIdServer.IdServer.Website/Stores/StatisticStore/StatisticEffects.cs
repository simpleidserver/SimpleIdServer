﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Statistics;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.StatisticStore
{
    public class StatisticEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;

        public StatisticEffects(
            IWebsiteHttpClientFactory websiteHttpClientFactory, 
            IOptions<IdServerWebsiteOptions> options)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
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
                var realm = RealmContext.Instance()?.Realm;
                var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
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
