﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website
{
    public interface IWebsiteHttpClientFactory
    {
        Task<HttpClient> Build(string realm = null);
        HttpClient Get();
    }

    public class WebsiteHttpClientFactory : IWebsiteHttpClientFactory
    {
        private readonly DefaultSecurityOptions _securityOptions;
        private readonly IdServerWebsiteOptions _idServerWebsiteOptions;
        private static SemaphoreSlim _lck = new SemaphoreSlim(1);
        private readonly HttpClient _httpClient;
        private readonly JsonWebTokenHandler _jsonWebTokenHandler;
        private GetAccessTokenResult _accessToken;

        public WebsiteHttpClientFactory(DefaultSecurityOptions securityOptions, IOptions<IdServerWebsiteOptions> idServerWebsiteOptions)
        {
            _securityOptions = securityOptions;
            _idServerWebsiteOptions = idServerWebsiteOptions.Value;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };
            _httpClient = new HttpClient(handler);
            _jsonWebTokenHandler = new JsonWebTokenHandler();
        }

        public async Task<HttpClient> Build(string realm = null)
        {
            var token = await GetAccessToken(realm);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
            var acceptLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (_httpClient.DefaultRequestHeaders.Contains("Language"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Language");
            }

            _httpClient.DefaultRequestHeaders.Add("Language", acceptLanguage);
            return _httpClient;
        }

        public HttpClient Get() => _httpClient;

        private async Task<GetAccessTokenResult> GetAccessToken(string realm = null)
        {
            await _lck.WaitAsync();
            if (_accessToken != null && _accessToken.IsValid)
            {
                _lck.Release();
                return _accessToken;
            }

            try
            {
                var content = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", _securityOptions.ClientId),
                    new KeyValuePair<string, string>("client_secret", _securityOptions.ClientSecret),
                    new KeyValuePair<string, string>("scope", "provisioning users acrs configurations authenticationschemeproviders authenticationmethods registrationworkflows apiresources auditing certificateauthorities clients realms groups scopes federation_entities"),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                };
                var url = _securityOptions.Issuer;
                if (!string.IsNullOrWhiteSpace(realm))
                    url += $"/{realm}";
                url += "/token";
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new FormUrlEncodedContent(content)
                };
                _httpClient.DefaultRequestHeaders.Clear();
                var httpResult = await _httpClient.SendAsync(httpRequest);
                var json = await httpResult.Content.ReadAsStringAsync();
                var accessToken = JsonObject.Parse(json)["access_token"].GetValue<string>();
                JsonWebToken jwt = null;
                if (_jsonWebTokenHandler.CanReadToken(accessToken))
                {
                    jwt = _jsonWebTokenHandler.ReadJsonWebToken(accessToken);
                }

                _accessToken = new GetAccessTokenResult(accessToken, jwt);
                return _accessToken;
            }
            finally
            {
                _lck.Release();
            }
        }

        private record GetAccessTokenResult
        {
            public GetAccessTokenResult(string accessToken, JsonWebToken jwt)
            {
                AccessToken = accessToken;
                Jwt = jwt;
            }

            public string AccessToken { get; private set; }
            public JsonWebToken Jwt { get; private set; }
            public bool IsValid => Jwt != null && Jwt.ValidTo >= DateTime.UtcNow;
        }
    }
}
