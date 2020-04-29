// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Gateway.Host.Handlers
{
    public class BaseClientCredentialsHandler : DelegatingHandler
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _oauthBaseUrl;
        private readonly IEnumerable<string> _scopes;

        public BaseClientCredentialsHandler(IConfiguration configuration, IEnumerable<string> scopes)
        {
            _clientId = configuration["ClientId"];
            _clientSecret = configuration["ClientSecret"];
            _oauthBaseUrl = configuration["OAuthBaseUrl"];
            _scopes = scopes;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("Authorization"))
            {
                request.Headers.Remove("Authorization");
            }

            var accessToken = await GetAccessToken(_scopes);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetAccessToken(IEnumerable<string> scopes)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_oauthBaseUrl}/token"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", _clientId },
                        { "client_secret", _clientSecret },
                        { "grant_type", "client_credentials" },
                        { "scope", string.Join(" ", scopes) }
                    })
                };
                var httpResult = await httpClient.SendAsync(request);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(json);
                return jObj["access_token"].ToString();
            }
        }
    }
}
