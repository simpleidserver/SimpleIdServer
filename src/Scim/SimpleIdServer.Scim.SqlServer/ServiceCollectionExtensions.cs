// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim.Infrastructure.Lock;
using SimpleIdServer.Scim.SqlServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedLockSQLServer(this IServiceCollection services, Action<SqlDistributedLockOptions> callback)
        {
            services.AddTransient<IDistributedLock, SqlDistributedLock>();
            services.Configure(callback);
            return services;
        }
    }
}
