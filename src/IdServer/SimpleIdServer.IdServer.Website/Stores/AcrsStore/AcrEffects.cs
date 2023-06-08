// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore
{
    public class AcrEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AcrEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }


        [EffectMethod]
        public async Task Handle(GetAllAcrsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/acrs")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var acrs = JsonSerializer.Deserialize<IEnumerable<AuthenticationContextClassReference>>(json);
            dispatcher.Dispatch(new GetAllAcrsSuccessAction { Acrs = acrs });
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class GetAllAcrsAction
    {

    }

    public class GetAllAcrsSuccessAction
    {
        public IEnumerable<AuthenticationContextClassReference> Acrs { get; set; }
    }
}
