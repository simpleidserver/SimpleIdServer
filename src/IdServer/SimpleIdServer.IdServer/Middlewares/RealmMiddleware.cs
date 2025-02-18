// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares
{
    public class RealmMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RealmMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RealmMiddleware(RequestDelegate next, ILogger<RealmMiddleware> logger, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, IRealmStore realmStore)
        {
            realmStore.Realm = null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var realmRepository = scope.ServiceProvider.GetRequiredService<IRealmRepository>();
                var routeValues = context.Request.RouteValues;
                string realm = string.Empty;
                var realmCookie = context.Request.Cookies.FirstOrDefault(c => c.Key == Constants.DefaultRealmCookieName);
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
                                    Constants.DefaultRealmCookieName,
                                    realm);
                            }
                        }
                    }
                    else realm = prefix;
                }

                if (string.IsNullOrWhiteSpace(realm) && !realmCookie.Equals(default(KeyValuePair<string, string>)) && !string.IsNullOrWhiteSpace(realmCookie.Value))
                    realm = realmCookie.Value;

                realmStore.Realm = realm;
            }

            await _next.Invoke(context);
        }
    }
}
