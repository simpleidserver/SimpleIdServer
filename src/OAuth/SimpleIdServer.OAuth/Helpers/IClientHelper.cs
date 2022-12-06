// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IClientHelper
    {
        Task<IEnumerable<string>> GetRedirectionUrls(Client client, CancellationToken cancellationToken);
        Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client);
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

        public async Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client)
        {
            if (client.JsonWebKeys != null && client.JsonWebKeys.Any())
            {
                IEnumerable<JsonWebKey> res = client.JsonWebKeys;
                return res;
            }

            Uri uri = null;
            if (string.IsNullOrWhiteSpace(client.JwksUri) || !Uri.TryCreate(client.JwksUri, UriKind.Absolute, out uri))
            {
                return new JsonWebKey[0];
            }

            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                httpClient.BaseAddress = uri;
                var request = await httpClient.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);
                request.EnsureSuccessStatusCode();
                var json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                var keysJson = JsonObject.Parse(json)["keys"].AsArray();
                var jsonWebKeys = keysJson.Select(k => JsonWebKey.Deserialize(k.ToString()));
                return jsonWebKeys;
            }
        }
    }
}
