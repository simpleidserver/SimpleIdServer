// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Store.SqlSugar;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder UseSqlSugarStore(this IdServerBuilder builder, Action<SqlSugarOptions> callback)
    {
        builder.Services.AddSqlSugarStore(callback);
        return builder;
    }
}
