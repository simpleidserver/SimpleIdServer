// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Client
{
    public static class OAuthClientExtensions
    {
        public static async Task<OAuthHttpResult> GetUserInfo(this OAuthClient oauthClient, string token)
        {
            var discovery = await oauthClient.GetDiscovery().ConfigureAwait(false);
            var userInfoUri = discovery.Content["userinfo_endpoint"].ToString();
            var httpClient = oauthClient.HttpClientFactory.GetHttpClient();
            var httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(userInfoUri),
                Method = HttpMethod.Get
            };
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
            var httpResult = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            var serializedContent = await httpResult.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new OAuthHttpResult(httpResult.StatusCode, JsonConvert.DeserializeObject<JObject>(serializedContent));
        }
    }
}