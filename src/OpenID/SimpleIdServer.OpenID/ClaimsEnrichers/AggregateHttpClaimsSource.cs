// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public interface IClaimsSource
    {
        Task Enrich(Dictionary<string, object> claims, Client client, CancellationToken cancellationToken);
    }

    public class AggregateHttpClaimsSource : IClaimsSource
    {
        private readonly AggregateHttpClaimsSourceOptions _httpClaimsSourceOptions;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly OAuth.Infrastructures.IHttpClientFactory _httpClientFactory;

        public AggregateHttpClaimsSource(AggregateHttpClaimsSourceOptions httpClaimsSourceOptions, IJwtBuilder jwtBuilder, OAuth.Infrastructures.IHttpClientFactory httpClientFactory)
        {
            _httpClaimsSourceOptions = httpClaimsSourceOptions;
            _jwtBuilder = jwtBuilder;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Enrich(Dictionary<string, object> claims, Client client, CancellationToken cancellationToken)
        {
            var subject = claims[JwtRegisteredClaimNames.Sub];
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(_httpClaimsSourceOptions.Url),
                    Method = HttpMethod.Get
                };
                if (!string.IsNullOrWhiteSpace(_httpClaimsSourceOptions.ApiToken))
                    request.Headers.Add("Authorization", $"Bearer {_httpClaimsSourceOptions.ApiToken}");

                var result = await httpClient.SendAsync(request);
                var json = await result.Content.ReadAsStringAsync();
                var kvp = result.Content.Headers.FirstOrDefault(k => k.Key == "Content-Type");
                if (!kvp.Equals(default(KeyValuePair<string, IEnumerable<string>>)) && !string.IsNullOrWhiteSpace(kvp.Key) && result.IsSuccessStatusCode)
                {
                    string jwt = null;
                    JsonObject jObj = null;
                    if (kvp.Value.Any(v => v.Contains("application/json")))
                    {
                        var securityTokenDescriptor = new SecurityTokenDescriptor
                        {
                            Claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                        };
                        jObj = JsonSerializer.SerializeToNode(json).AsObject();
                        jwt = await _jwtBuilder.BuildClientToken(client, securityTokenDescriptor, client.GetIdTokenSignedResponseAlg(), client.GetIdTokenEncryptedResponseAlg(), client.GetIdTokenEncryptedResponseEnc(), cancellationToken);
                    }
                    else if (kvp.Value.Any(v => v.Contains("application/jwt")))
                        throw new NotImplementedException();

                    if (!string.IsNullOrWhiteSpace(jwt))
                        ClaimsSourceHelper.AddAggregate(claims, jObj, jwt, _httpClaimsSourceOptions.Name);
                }
            }
        }
    }

    public class AggregateHttpClaimsSourceOptions
    {
        public AggregateHttpClaimsSourceOptions(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public AggregateHttpClaimsSourceOptions(string name, string url, string apiToken) : this(name, url)
        {
            ApiToken = apiToken;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ApiToken { get; private set; }
    }
}
