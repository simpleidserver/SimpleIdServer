// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IClientHelper
    {
        Task<IEnumerable<string>> GetRedirectionUrls(Client client, CancellationToken cancellationToken);
        Task<JsonWebKey> ResolveJsonWebKey(Client client, string kid, CancellationToken cancellationToken);
        Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client, CancellationToken cancellationToken);
    }

    public class OAuthClientHelper : IClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OAuthClientHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public virtual Task<IEnumerable<string>> GetRedirectionUrls(Client client, CancellationToken cancellationToken)
        {
            IEnumerable<string> result = client.RedirectionUrls == null ? new List<string>() : client.RedirectionUrls;
            return Task.FromResult(result);
        }

        public async Task<JsonWebKey> ResolveJsonWebKey(Client client, string kid, CancellationToken cancellationToken)
        {
            var jwks = await ResolveJsonWebKeys(client, cancellationToken);
            return jwks.FirstOrDefault(j => j.KeyId == kid);
        }

        public async Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client, CancellationToken cancellationToken)
        {
            if (client.JsonWebKeys != null && client.JsonWebKeys.Any()) return client.JsonWebKeys;
            Uri uri = null;
            if (string.IsNullOrWhiteSpace(client.JwksUri) || !Uri.TryCreate(client.JwksUri, UriKind.Absolute, out uri)) return new JsonWebKey[0];
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                httpClient.BaseAddress = uri;
                var request = await httpClient.GetAsync(uri.AbsoluteUri, cancellationToken).ConfigureAwait(false);
                request.EnsureSuccessStatusCode();
                var json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                var keysJson = JsonObject.Parse(json)["keys"].AsArray();
                var jsonWebKeys = keysJson.Select(k => JsonExtensions.DeserializeFromJson<JsonWebKey>(k.ToString()));
                return jsonWebKeys;
            }
        }
    }
}
