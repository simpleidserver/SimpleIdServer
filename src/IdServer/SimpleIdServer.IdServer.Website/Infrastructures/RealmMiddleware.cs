// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
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
        "/auth",
        "/callback",
        "/signout-callback-oidc",
        "/signin-oidc",
        "/oidccallback",
        "/bc-logout",
        "/availablerealms"
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

    public RealmMiddleware(
        RequestDelegate next,
        IOptions<IdServerWebsiteOptions> options,
        ILogger<RealmMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRealmStore realmStore, IWebsiteHttpClientFactory websiteHttpClientFactory)
    {
        var path = context.Request.Path.Value;
        if (_excludedFileExtensions.Any(r => path.EndsWith(r))
            || _excludedRoutes.Any(r => path.StartsWith(r))
            || !_options.IsReamEnabled)
        {
            await _next.Invoke(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            ReturnNotFound(context);
            return;
        }

        var splitted = path.Split('/').Where(p => !string.IsNullOrWhiteSpace(p));
        if (splitted.Count() < 2)
        {
            ReturnNotFound(context);
            return;
        }

        var currentRealm = splitted.First();
        var existingRealms = await GetRealms(currentRealm, websiteHttpClientFactory);
        if (!existingRealms.Any(r => r.Name == currentRealm))
        {
            EnsureCookiesAreRemoved(context, currentRealm);
            var redirectUrl = $"{context.Request.GetAbsoluteUriWithVirtualPath()}/availablerealms";
            context.Response.Redirect(redirectUrl);
            return;
        }

        realmStore.Realm = currentRealm;
        context.Response.Cookies.Append(
            CookieRealmStore.DefaultRealmCookieName,
            currentRealm);
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

    private async Task<IEnumerable<Realm>> GetRealms(string currentRealm, IWebsiteHttpClientFactory websiteHttpClientFactory)
    {
        try
        {
            var url = await GetBaseUrl(currentRealm);
            var httpClient = await websiteHttpClientFactory.Build(currentRealm);
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
        catch (Exception ex)
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