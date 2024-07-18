// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenidFederation.Store.EF;
using SimpleIdServer.OpenidFederation.Store.EF.Stores;
using SimpleIdServer.OpenidFederation.Stores;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenidFederationStore(this IServiceCollection services, Action<DbContextOptionsBuilder> cb = null)
    {
        if (cb != null) services.AddDbContext<OpenidFederationDbContext>(cb);
        else services.AddDbContext<OpenidFederationDbContext>(c => c.UseInMemoryDatabase("openidFederation"));
        services.AddTransient<IFederationEntityStore, FederationEntityStore>();
        return services;
    }
}