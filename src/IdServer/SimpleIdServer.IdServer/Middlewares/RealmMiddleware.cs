// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares;

public class RealmMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private static List<string> _includedPathLst = new List<string>
    {
        "/signin-"
    };

    public RealmMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context, IRealmStore realmStore)
    {
        realmStore.Realm = null;
        using (var scope = _serviceProvider.CreateScope())
        {
            var realmRepository = scope.ServiceProvider.GetRequiredService<IRealmRepository>();
            var routeValues = context.Request.RouteValues;
            var realm = string.Empty;
            var realmCookie = context.Request.Cookies.FirstOrDefault(c => c.Key == CookieRealmStore.DefaultRealmCookieName);
            if (routeValues.ContainsKey(Constants.Prefix))
            {
                var prefix = routeValues.First(v => v.Key == Constants.Prefix).Value?.ToString();
                if (realmCookie.Value != prefix)
                {
                    var existingRealm = await realmRepository.Get(prefix, CancellationToken.None);
                    if (existingRealm != null)
                    {
                        realm = prefix;
                        if (!string.IsNullOrWhiteSpace(realm))
                        {
                            context.Response.Cookies.Append(
                                CookieRealmStore.DefaultRealmCookieName,
                                realm);
                        }
                    }
                    else if (context.Request.Path.HasValue && !_includedPathLst.Any(p => context.Request.Path.Value.StartsWith(p)))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }
                }
                else
                {
                    realm = prefix;
                }
            }

            if (string.IsNullOrWhiteSpace(realm) && !realmCookie.Equals(default(KeyValuePair<string, string>)) && !string.IsNullOrWhiteSpace(realmCookie.Value))
                realm = realmCookie.Value;

            realmStore.Realm = realm;
        }

        await _next.Invoke(context);
    }
}