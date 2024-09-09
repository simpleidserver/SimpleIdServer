// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Webfinger.Client;

public class WebfingerClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebfingerClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GetWebfingerResult> Get(string url, GetWebfingerRequest request, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var result = await httpClient.GetFromJsonAsync<GetWebfingerResult>($"{url}?{request.ToQueryParameters()}", cancellationToken);
            return result;
        }
    }
}
