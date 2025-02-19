// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRealmStore(this IServiceCollection services, bool readCookie)
    {
        if(readCookie)
        {
            services.AddScoped<IRealmStore, CookieRealmStore>();
        }
        else
        {
            services.AddScoped<IRealmStore, RealmStore>();
        }

        return services;
    }
}
