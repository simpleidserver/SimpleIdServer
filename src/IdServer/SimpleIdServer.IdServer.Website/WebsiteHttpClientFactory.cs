// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website
{
    public interface IWebsiteHttpClientFactory
    {
        Task<HttpClient> Build();
        HttpClient Get();
    }

    public class WebsiteHttpClientFactory : IWebsiteHttpClientFactory
    {
        private readonly DefaultSecurityOptions _securityOptions;
        private readonly HttpClient _httpClient;
        private readonly JsonWebTokenHandler _jsonWebTokenHandler;
        private GetAccessTokenResult _accessToken;

        public WebsiteHttpClientFactory(DefaultSecurityOptions securityOptions)
        {
            _securityOptions = securityOptions;
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

        public async Task<HttpClient> Build()
        {
            var token = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
            var acceptLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", acceptLanguage);
            return _httpClient;   
        }

        public HttpClient Get() => _httpClient;

        private async Task<GetAccessTokenResult> GetAccessToken()
        {
            if (_accessToken != null && _accessToken.IsValid) return _accessToken;
            var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _securityOptions.ClientId),
                new KeyValuePair<string, string>("client_secret", _securityOptions.ClientSecret),
                new KeyValuePair<string, string>("scope", "provisioning networks users credential_offer acrs configurations authenticationschemeproviders authenticationmethods registrationworkflows apiresources auditing certificateauthorities clients credential_templates realms groups scopes"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_securityOptions.Issuer}/token"),
                Content = new FormUrlEncodedContent(content)
            };
            _httpClient.DefaultRequestHeaders.Clear();
            var httpResult = await _httpClient.SendAsync(httpRequest);
            var json = await httpResult.Content.ReadAsStringAsync();
            var accessToken = JsonObject.Parse(json)["access_token"].GetValue<string>();
            JsonWebToken jwt = null;
            if(_jsonWebTokenHandler.CanReadToken(accessToken))
            {
                jwt = _jsonWebTokenHandler.ReadJsonWebToken(accessToken);
            }

            _accessToken = new GetAccessTokenResult(accessToken, jwt);
            return _accessToken;
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
