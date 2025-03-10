// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Store.EF;

namespace SimpleIdServer.IdServer;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder UseInMemoryEFStore(this IdServerBuilder builder, Action<IdServerInMemoryStoreBuilder> callback = null)
    {
        builder.Services.AddEfStore();
        builder.FormBuilder.UseEF();
        var serviceProvider = builder.Services.BuildServiceProvider();
        if (callback != null)
        {
            callback(new IdServerInMemoryStoreBuilder(serviceProvider));
        }

        if (builder.DataProtectionBuilder != null)
        {
            builder.DataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
        }

        return builder;
    }

    public static IdServerBuilder UseEfStore(this IdServerBuilder builder, Action<DbContextOptionsBuilder> dbCallback)
    {
        builder.Services.AddEfStore(dbCallback);
        if (builder.DataProtectionBuilder != null)
        {
            builder.DataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
        }

        return builder;
    }
}
