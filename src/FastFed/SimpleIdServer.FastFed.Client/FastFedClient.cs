// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.IdServer.Helpers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Client;

public class FastFedClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FastFedClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ProviderMetadata> GetProviderMetadata(string baseUrl, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var providerMetadata = await httpClient.GetFromJsonAsync<ProviderMetadata>($"{baseUrl}/provider-metadata", cancellationToken);
            return providerMetadata;
        }
    }
}