// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SimpleIdServer.WsFederation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWsFederation(this IServiceCollection services, Action<WsFederationOptions>? callback = null)
        {
            if (callback == null) services.Configure<WsFederationOptions>(c => { });
            else services.Configure(callback);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            return services;
        }
    }
}
