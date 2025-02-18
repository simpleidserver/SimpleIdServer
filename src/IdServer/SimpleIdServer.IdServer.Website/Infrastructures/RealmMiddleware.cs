// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Net;

namespace SimpleIdServer.IdServer.Website.Middlewares;

public class RealmMiddleware
{
    private static List<string> _excludedRoutes = new List<string>
    {
        "/_blazor",
        "/logout",
        "/Culture",
        "/login",
        "/callback",
        "/signout-callback-oidc",
        "/oidccallback",
        "/bc-logout"
    };
    private static List<string> _excludedFileExtensions = new List<string>
    {
        ".js",
        ".css",
        ".woff"
    };
    private readonly RequestDelegate _next;
    private readonly IdServerWebsiteOptions _options;
    private readonly ILogger<RealmMiddleware> _logger;
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;

    public RealmMiddleware(
        RequestDelegate next, 
        IOptions<IdServerWebsiteOptions> options, 
        ILogger<RealmMiddleware> logger,
        IWebsiteHttpClientFactory websiteHttpClientFactory)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
        _websiteHttpClientFactory = websiteHttpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context, IRealmStore realmStore)
    {
        var path = context.Request.Path.Value;
        if(_excludedFileExtensions.Any(r => path.EndsWith(r)) 
            || _excludedRoutes.Any(r => path.StartsWith(r))
            || !_options.IsReamEnabled)
        {
            await _next.Invoke(context);
            return;
        }

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
        var existingRealms = await GetRealms(currentRealm);
        if(!existingRealms.Any(r => r.Name == currentRealm))
        {
            EnsureCookiesAreRemoved(context, currentRealm);
            ReturnNotFound(context);
            return;
        }

        realmStore.Realm = currentRealm;
        await _next.Invoke(context);
    }

    private void EnsureCookiesAreRemoved(HttpContext context, string currentRealm)
    {
        var cookieName = $"{CookieAuthenticationDefaults.CookiePrefix + Uri.EscapeDataString("AdminWebsite")}.{currentRealm}";
        var filteredCookieNames = context.Request.Cookies.Where(c => c.Key.StartsWith(cookieName)).Select(c => c.Key);
        foreach (var cookie in filteredCookieNames)
        {
            context.Response.Cookies.Delete(cookie);
        }
    }

    private void ReturnNotFound(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
    }

    private async Task<IEnumerable<Realm>> GetRealms(string currentRealm)
    {
        try
        {
            var url = await GetBaseUrl(currentRealm);
            var httpClient = await _websiteHttpClientFactory.Build(currentRealm);
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
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return new List<Realm>();
        }
    }

    private async Task<string> GetBaseUrl(string realm)
    {
        if (_options.IsReamEnabled)
        {
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/realms";
        }

        return $"{_options.IdServerBaseUrl}/realms";
    }
}
