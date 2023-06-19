// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Website.Stores.IdServerConfigurationStore
{
    public class IdServerConfigurationEffects
    {
        private readonly IWebsiteHttpClientFactory _httpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public IdServerConfigurationEffects(IWebsiteHttpClientFactory httpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(GetIdServerConfigurationAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = _httpClientFactory.Get();
            var configuration = await httpClient.GetFromJsonAsync<IdServerConfigurationResult>($"{_options.IdServerBaseUrl}/{realm}/.well-known/idserver-configuration");
            dispatcher.Dispatch(new GetIdServerConfigurationSuccessAction(configuration));
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class GetIdServerConfigurationAction
    {

    }

    public class GetIdServerConfigurationSuccessAction
    {
        public GetIdServerConfigurationSuccessAction(IdServerConfigurationResult configuration)
        {
            Configuration = configuration;
        }

        public IdServerConfigurationResult Configuration { get; private set; }
    }

    public class IdServerConfigurationResult
    {
        [JsonPropertyName("amrs")]
        public IEnumerable<string> Amrs { get; set; }
    }
}
