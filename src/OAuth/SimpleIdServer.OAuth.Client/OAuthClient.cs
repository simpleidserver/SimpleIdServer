// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Client.Selectors;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Client
{
    public class OAuthClient
    {
        private readonly Uri _oauthConfigurationUri;
        private readonly IHttpClientFactory _httpClientFactory;

        public OAuthClient(string oauthConfigurationUrl)
        {
            _oauthConfigurationUri = new Uri(oauthConfigurationUrl);
            _httpClientFactory = new HttpClientFactory();
        }

        public IHttpClientFactory HttpClientFactory => _httpClientFactory;

        public OAuthClient(string oauthConfigurationUrl, IHttpClientFactory httpClientFactory) : this(oauthConfigurationUrl)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task<OAuthHttpResult> GetDiscovery()
        {
            return InternalHttpGet(_oauthConfigurationUri);
        }

        public async Task<OAuthHttpResult> GetJwks()
        {
            var discovery = await GetDiscovery().ConfigureAwait(false);
            return await InternalHttpGet(new Uri(discovery.Content["jwks_uri"].ToString())).ConfigureAwait(false);
        }

        public IClientAuthSelector ClientAuth
        {
            get => new ClientAuthSelector(_httpClientFactory, new TokenRequestBuilder(GetDiscovery));
        }

        private async Task<OAuthHttpResult> InternalHttpGet(Uri uri)
        {
            var httpClient = _httpClientFactory.GetHttpClient();
            var httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = _oauthConfigurationUri,
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            var serializedContent = await httpResult.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new OAuthHttpResult(httpResult.StatusCode, JsonConvert.DeserializeObject<JObject>(serializedContent));
        }
    }
}
