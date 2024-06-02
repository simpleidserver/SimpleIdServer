// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SqlSugarOptions
{
    public ConnectionConfig ConnectionConfig { get; set; }
    public DbType DbType { get; set; } = DbType.SqlServer;
}