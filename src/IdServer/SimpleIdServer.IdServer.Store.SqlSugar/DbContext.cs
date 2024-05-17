// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class DbContext : IDisposable
{
    private readonly SqlSugarClient _client;

    public DbContext(IOptions<SqlSugarOptions> options)
    {
        var connectionConfig = options.Value.ConnectionConfig;
        connectionConfig.IsAutoCloseConnection = true;
        connectionConfig.DbType = DbType.SqlServer;
        _client = new SqlSugarClient(connectionConfig, it =>
        {
            it.Aop.OnLogExecuted = (sql, para) =>
            {
                var ss = UtilMethods.GetNativeSql(sql, para);

                string ss2 = "";
            };
        });
    }

    public SqlSugarClient Client
    {
        get
        {
            return _client;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
