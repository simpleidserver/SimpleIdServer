// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class DbContext : IDisposable
{
    private readonly SqlSugarScope _client;

    public DbContext(IOptions<SqlSugarOptions> options)
    {
        var connectionConfig = options.Value.ConnectionConfig;
        connectionConfig.IsAutoCloseConnection = true;
        connectionConfig.DbType = DbType.SqlServer;
        _client = new SqlSugarScope(connectionConfig, it =>
        {
            it.Aop.OnLogExecuted = (sql, para) =>
            {
                var ss = UtilMethods.GetNativeSql(sql, para);
                string ss2 = "";
            };
        });
        UserSessions = new SimpleClient<SugarUserSession>(_client);
        Users = new SimpleClient<SugarUser>();
    }

    public SimpleClient<SugarUserSession> UserSessions { get; set; }
    public SimpleClient<SugarUser> Users { get; set; }

    public SqlSugarScope Client
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

    public Guid Id { get; set; }
}
