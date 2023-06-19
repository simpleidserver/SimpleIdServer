// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Ethr.Store;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDIDEthrStore(this IServiceCollection services, Action<DbContextOptionsBuilder>? action = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddTransient<IIdentityDocumentConfigurationStore, EFIdentityDocumentConfigurationStore>();
            if (action != null) services.AddDbContext<EthrDbContext>(action, lifetime);
            else services.AddDbContext<EthrDbContext>(o => o.UseInMemoryDatabase("ethrDid"), lifetime);
            return services;
        }
    }
}
