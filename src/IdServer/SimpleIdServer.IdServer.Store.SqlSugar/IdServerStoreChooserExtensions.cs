// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Store.SqlSugar;
using SimpleIdServer.IdServer.Store.SqlSugar.Seeding;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerStoreChooserExtensions
{
    public static IdServerBuilder UseSqlSugar(this IdServerStoreChooser builder, Action<SqlSugarOptions> callback)
    {
        builder.Services.AddSqlSugarStore(callback);
        builder.Services.AddEntitySeeders(typeof(UserEntitySeeder));
        return new IdServerBuilder(builder.Services, builder.AuthBuilder);
    }
}
