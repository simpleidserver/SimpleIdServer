// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Store.EF;

namespace SimpleIdServer.IdServer;

public static class IdServerStoreChooserExtensions
{
    public static IdServerBuilder UseEFStore(this IdServerBuilder builder, Action<DbContextOptionsBuilder> dbCallback)
    {
        builder.Services.AddEfStore(dbCallback);
        return builder;
    }

    public static IdServerBuilder UseInMemoryEFStore(this IdServerBuilder builder, Action<IdServerInMemoryStoreBuilder> callback = null)
    {
        builder.Services.AddEfStore();
        builder.FormBuilder.UseEF();
        var serviceProvider = builder.Services.BuildServiceProvider();
        if(callback != null)
        {
            callback(new IdServerInMemoryStoreBuilder(serviceProvider));
        }

        return builder;
    }
}
