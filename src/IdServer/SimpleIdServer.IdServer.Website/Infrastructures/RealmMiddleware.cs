// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
using System.Net;

namespace SimpleIdServer.IdServer.Website.Middlewares;

public class RealmMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IdServerWebsiteOptions _options;
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;

    public RealmMiddleware(RequestDelegate next, IOptions<IdServerWebsiteOptions> options, IWebsiteHttpClientFactory websiteHttpClientFactory)
    {
        _next = next;
        _options = options.Value;
        _websiteHttpClientFactory = websiteHttpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var routeValues = context.Request.RouteValues;
        var path = context.Request.Path.Value;
        if(string.IsNullOrWhiteSpace(path))
        {
            ReturnNotFound(context);
            return;
        }

        var splitted = path.Split('/').Where(p => !string.IsNullOrWhiteSpace(p));
        if(splitted.Count() < 2)
        {
            ReturnNotFound(context);
            return;
        }

        var currentRealm = splitted.First();
        var existingRealms = await GetRealms();
        if(!existingRealms.Any(r => r.Name == currentRealm))
        {
            ReturnNotFound(context);
            return;
        }

        RealmContext.Instance().Realm = currentRealm;
        await _next.Invoke(context);
    }

    private void ReturnNotFound(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
    }

    private async Task<IEnumerable<Realm>> GetRealms()
    {
        var url = $"{_options.IdServerBaseUrl}/realms";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var realms = SidJsonSerializer.Deserialize<IEnumerable<Realm>>(json);
        return realms;
    }
}
