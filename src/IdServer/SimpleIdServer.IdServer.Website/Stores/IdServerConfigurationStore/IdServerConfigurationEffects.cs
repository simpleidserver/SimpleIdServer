// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Website.Infrastructures;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Website.Stores.IdServerConfigurationStore
{
    public class IdServerConfigurationEffects
    {
        private readonly IWebsiteHttpClientFactory _httpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly CurrentRealm _currentRealm;

        public IdServerConfigurationEffects(
            IWebsiteHttpClientFactory httpClientFactory, 
            IOptions<IdServerWebsiteOptions> options, 
            CurrentRealm currentRealm)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _currentRealm = currentRealm;
        }

        [EffectMethod]
        public async Task Handle(GetIdServerConfigurationAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = _httpClientFactory.Get();
            var configuration = await httpClient.GetFromJsonAsync<IdServerConfigurationResult>($"{baseUrl}/.well-known/idserver-configuration");
            dispatcher.Dispatch(new GetIdServerConfigurationSuccessAction(configuration));
        }

        [EffectMethod]
        public async Task Handle(GetOpenIdServerConfigurationAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = _httpClientFactory.Get();
            var configuration = await httpClient.GetFromJsonAsync<OpenIdServerConfigurationResult>($"{baseUrl}/.well-known/openid-configuration");
            dispatcher.Dispatch(new GetOpenIdServerConfigurationSuccessAction(configuration));
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realmStr = !string.IsNullOrWhiteSpace(_currentRealm.Identifier) ? _currentRealm.Identifier: SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}";
            }

            return _options.IdServerBaseUrl;
        }
    }

    public class GetIdServerConfigurationAction
    {

    }

    public class GetOpenIdServerConfigurationAction
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

    public class GetOpenIdServerConfigurationSuccessAction
    {
        public GetOpenIdServerConfigurationSuccessAction(OpenIdServerConfigurationResult configuration)
        {
            Configuration = configuration;
        }

        public OpenIdServerConfigurationResult Configuration { get; private set; }
    }

    public class IdServerConfigurationResult
    {
        [JsonPropertyName("amrs")]
        public IEnumerable<string> Amrs { get; set; }
    }

    public class OpenIdServerConfigurationResult
    {
        [JsonPropertyName("response_types_supported")]
        public IEnumerable<string> ResponseTypesSupported { get; set; }
    }
}
