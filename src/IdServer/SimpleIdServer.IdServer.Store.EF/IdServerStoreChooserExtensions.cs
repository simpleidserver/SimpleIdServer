// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Store.EF;

namespace SimpleIdServer.IdServer;

public static class IdServerStoreChooserExtensions
{
    public static IdServerBuilder UseEFStore(this IdServerStoreChooser builder, Action<DbContextOptionsBuilder> dbCallback)
    {
        builder.Services.AddEFStore(dbCallback);
        return new IdServerBuilder(builder.Services, builder.AuthBuilder);
    }

    public static IdServerBuilder UseInMemoryEFStore(this IdServerStoreChooser builder, Action<IdServerInMemoryStoreBuilder> callback)
    {
        builder.Services.AddEFStore();
        var serviceProvider = builder.Services.BuildServiceProvider();
        callback(new IdServerInMemoryStoreBuilder(serviceProvider));
        return new IdServerBuilder(builder.Services, builder.AuthBuilder);
    }
}
