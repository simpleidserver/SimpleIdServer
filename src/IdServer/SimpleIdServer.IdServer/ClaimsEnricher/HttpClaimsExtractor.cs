// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.ClaimsEnricher
{
    /// <summary>
    /// https://openid.net/specs/openid-connect-core-1_0.html#AggregatedDistributedClaims
    /// </summary>
    public class HttpClaimsExtractor : IRelayClaimsExtractor
    {
        private readonly Infrastructures.IHttpClientFactory _httpClientFactory;

        public HttpClaimsExtractor(Infrastructures.IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string ProviderType => throw new NotImplementedException();

        public async Task<AggregatedClaimsExtractionResult> ExtractAggregatedClaims(User user, string connectionString, CancellationToken cancellationToken)
        {
            var httpClaimsExtractorConn = new ClaimsExtractorConnectionStringSerializer().Deserialize<HttpClaimsExtractorConnectionString>(connectionString);
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var url = GetUrl(httpClaimsExtractorConn.Url, user);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                if (!string.IsNullOrWhiteSpace(httpClaimsExtractorConn.AccessToken))
                    request.Headers.Add("Authorization", $"Bearer {httpClaimsExtractorConn.AccessToken}");

                var result = await httpClient.SendAsync(request);
                var json = await result.Content.ReadAsStringAsync();
                var kvp = result.Content.Headers.FirstOrDefault(k => k.Key == "Content-Types");
                if (!kvp.Equals(default(KeyValuePair<string, IEnumerable<string>>)) && !string.IsNullOrWhiteSpace(kvp.Key) && result.IsSuccessStatusCode)
                {
                    if (kvp.Value.Any(v => v.Contains("application/jwt")))
                    {
                        var handler = new JsonWebTokenHandler();
                        var jwt = handler.ReadJsonWebToken(json);
                        return new AggregatedClaimsExtractionResult { ClaimNames = jwt.Claims.Select(c => c.Type), Jwt = json };
                    }
                }

                return null;
            }
        }

        public async Task<DistributedClaimsExtractionResult> ExtractDistributedClaims(User user, string connectionString, CancellationToken cancellationToken)
        {
            var httpClaimsExtractorConn = new ClaimsExtractorConnectionStringSerializer().Deserialize<HttpClaimsExtractorConnectionString>(connectionString);
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var url = GetUrl(httpClaimsExtractorConn.Url, user);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                if (!string.IsNullOrWhiteSpace(httpClaimsExtractorConn.AccessToken))
                    request.Headers.Add("Authorization", $"Bearer {httpClaimsExtractorConn.AccessToken}");

                var result = await httpClient.SendAsync(request);
                var json = await result.Content.ReadAsStringAsync();
                var kvp = result.Content.Headers.FirstOrDefault(k => k.Key == "Content-Types");
                if (!kvp.Equals(default(KeyValuePair<string, IEnumerable<string>>)) && !string.IsNullOrWhiteSpace(kvp.Key) && result.IsSuccessStatusCode)
                {
                    if (kvp.Value.Any(v => v.Contains("application/json")))
                    {
                        var jsonObject = JsonSerializer.SerializeToNode(json).AsObject();
                        return new DistributedClaimsExtractionResult { AccessToken = httpClaimsExtractorConn.AccessToken, Endpoint = url };
                    }
                }

                return null;
            }
        }

        private static string GetUrl(string url, User user) => url.Replace("{0}", user.Name);
    }

    public class HttpClaimsExtractorConnectionString
    {
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }
}
