// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website
{
    public interface IWebsiteHttpClientFactory
    {
        Task<HttpClient> Build(string currentRealm = null);
        HttpClient Get();
        void RemoveAccessToken(string realm);
    }

    public class WebsiteHttpClientFactory : IWebsiteHttpClientFactory
    {
        private readonly IRealmStore _realmStore;
        private readonly DefaultSecurityOptions _securityOptions;
        private readonly IdServerWebsiteOptions _idServerWebsiteOptions;
        private readonly HttpClient _httpClient;
        private readonly JsonWebTokenHandler _jsonWebTokenHandler;
        private ConcurrentDictionary<string, GetAccessTokenResult> _accessTokens = new ConcurrentDictionary<string, GetAccessTokenResult>();

        public WebsiteHttpClientFactory(IRealmStore realmStore, DefaultSecurityOptions securityOptions, IOptions<IdServerWebsiteOptions> idServerWebsiteOptions)
        {
            _realmStore = realmStore;
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

        public async Task<HttpClient> Build(string currentRealm = null)
        {
            var token = await GetAccessToken(currentRealm);
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

        public void RemoveAccessToken(string realm)
        {
            _accessTokens.TryRemove(realm, out GetAccessTokenResult r);
        }

        private async Task<GetAccessTokenResult> GetAccessToken(string currentRealm = null)
        {
            var realm = currentRealm;
            if(string.IsNullOrWhiteSpace(realm))
                realm = _idServerWebsiteOptions.IsReamEnabled ? (_realmStore.Realm ?? Constants.DefaultRealm) : string.Empty;

            GetAccessTokenResult accessToken = null;
            if(_accessTokens.ContainsKey(realm))
                accessToken = _accessTokens[realm];
            if (accessToken != null && accessToken.IsValid) return accessToken;
            if (accessToken != null && !accessToken.IsValid) _accessTokens.TryRemove(realm, out GetAccessTokenResult r); 
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
            var at = JsonObject.Parse(json)["access_token"].GetValue<string>();
            JsonWebToken jwt = null;
            if (_jsonWebTokenHandler.CanReadToken(at)) jwt = _jsonWebTokenHandler.ReadJsonWebToken(at);            
            accessToken = new GetAccessTokenResult(at, jwt);
            _accessTokens.TryAdd(realm, accessToken);
            return accessToken;
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
