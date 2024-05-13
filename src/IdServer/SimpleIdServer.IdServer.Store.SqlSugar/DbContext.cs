// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class DbContext
{
    public DbContext()
    {
        var db = new SqlSugarClient(new ConnectionConfig
        {
            DbType = 
        });
    }
}
