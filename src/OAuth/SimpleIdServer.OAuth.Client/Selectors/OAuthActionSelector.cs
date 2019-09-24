// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Client.Selectors
{
    public interface IOAuthActionSelector
    {
        Task<OAuthHttpResult> UseClientCredentialsGrantType(params string[] scopes);
        Task<OAuthHttpResult> UsePasswordGrantType(string username, string password);
        Task<OAuthHttpResult> UseAuthorizationCodeGrantType(string code, string redirectUri);
        Task<OAuthHttpResult> UseRefreshTokenGrantType(string refresnToken);
        Task<OAuthHttpResult> RevokeToken(string token, string tokenTypeHint = null);
    }

    internal sealed class OAuthActionSelector : IOAuthActionSelector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenRequestBuilder _tokenRequestBuilder;

        public OAuthActionSelector(IHttpClientFactory httpClientFactory, TokenRequestBuilder tokenRequestBuilder)
        {
            _httpClientFactory = httpClientFactory;
            _tokenRequestBuilder = tokenRequestBuilder;
        }

        public async Task<OAuthHttpResult> UseClientCredentialsGrantType(params string[] scopes)
        {
            try
            {
                _tokenRequestBuilder.AddBody("grant_type", "client_credentials");
                _tokenRequestBuilder.AddBody("scope", string.Join(" ", scopes));
                return await InternalHandleGrantType().ConfigureAwait(false);
            }
            finally
            {
                _tokenRequestBuilder.RemoveFromBody("grant_type", "scope");
            }
        }

        public async Task<OAuthHttpResult> UsePasswordGrantType(string username, string password)
        {
            try
            {
                _tokenRequestBuilder.AddBody("grant_type", "password");
                _tokenRequestBuilder.AddBody("username", username);
                _tokenRequestBuilder.AddBody("password", password);
                return await InternalHandleGrantType().ConfigureAwait(false);
            }
            finally
            {
                _tokenRequestBuilder.RemoveFromBody("grant_type", "username", "password");
            }
        }

        public async Task<OAuthHttpResult> UseAuthorizationCodeGrantType(string code, string redirectUri)
        {
            try
            {
                _tokenRequestBuilder.AddBody("grant_type", "authorization_code");
                _tokenRequestBuilder.AddBody("code", code);
                _tokenRequestBuilder.AddBody("redirect_uri", redirectUri);
                return await InternalHandleGrantType().ConfigureAwait(false);
            }
            finally
            {
                _tokenRequestBuilder.RemoveFromBody("grant_type", "code", "redirect_uri");
            }
        }

        public async Task<OAuthHttpResult> UseRefreshTokenGrantType(string refreshToken)
        {
            try
            {
                _tokenRequestBuilder.AddBody("grant_type", "refresh_token");
                _tokenRequestBuilder.AddBody("refresh_token", refreshToken);
                return await InternalHandleGrantType().ConfigureAwait(false);
            }
            finally
            {
                _tokenRequestBuilder.RemoveFromBody("grant_type", "refresh_token");
            }
        }

        public async Task<OAuthHttpResult> RevokeToken(string token, string tokenTypeHint = null)
        {
            try
            {

                _tokenRequestBuilder.AddBody("token", token);
                if (!string.IsNullOrWhiteSpace(tokenTypeHint))
                {
                    _tokenRequestBuilder.AddBody("token_type_hint", tokenTypeHint);
                }

                return await InternalRevokeToken().ConfigureAwait(false);
            }
            finally
            {
                _tokenRequestBuilder.RemoveFromBody("token", "token_type_hint");
            }
        }

        private async Task<OAuthHttpResult> InternalHandleGrantType()
        {
            var discovery = await _tokenRequestBuilder.GetDiscovery().ConfigureAwait(false);
            var httpClient = _httpClientFactory.GetHttpClient();
            var body = new FormUrlEncodedContent(_tokenRequestBuilder.Body);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = body,
                RequestUri = new Uri(discovery.Content["token_endpoint"].ToString())
            };
            foreach(var kvp in _tokenRequestBuilder.Header)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }

            var result = await httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new OAuthHttpResult(result.StatusCode, JsonConvert.DeserializeObject<JObject>(content));
        }

        private async Task<OAuthHttpResult> InternalRevokeToken()
        {
            var discovery = await _tokenRequestBuilder.GetDiscovery().ConfigureAwait(false);
            var httpClient = _httpClientFactory.GetHttpClient();
            var body = new FormUrlEncodedContent(_tokenRequestBuilder.Body);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = body,
                RequestUri = new Uri(discovery.Content["revocation_endpoint"].ToString())
            };
            foreach (var kvp in _tokenRequestBuilder.Header)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }

            var result = await httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new OAuthHttpResult(result.StatusCode, JsonConvert.DeserializeObject<JObject>(content));
        }
    }
}
