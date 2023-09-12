// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration.Redis;

public class RedisKeyValueConnector : IKeyValueConnector
{
    private readonly ConnectionMultiplexer _connection;

    public RedisKeyValueConnector(string configuration)
    {
        _connection = ConnectionMultiplexer.Connect(configuration);
    }

    public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        var hashEntries = await db.HashGetAllAsync("*");
        return hashEntries.ToDictionary().ToDictionary(k => k.Key.ToString(), k => k.Value.ToString());
    }

    public async Task Set(string key, string value, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        await db.StringSetAsync(key, value);
    }
}
