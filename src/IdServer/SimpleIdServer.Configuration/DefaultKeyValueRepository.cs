// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration;

public class DefaultKeyValueRepository : IKeyValueRepository
{
    public readonly List<ConfigurationKeyPairValueRecord> _records;

    public DefaultKeyValueRepository()
    {
        _records = new List<ConfigurationKeyPairValueRecord>();
    }

    public Task AddOrUpdate(ConfigurationKeyPairValueRecord keyValue, CancellationToken cancellationToken)
    {
        var record = _records.SingleOrDefault(r => r.Name == keyValue.Name);
        if (record == null)
        {
            record = new ConfigurationKeyPairValueRecord
            {
                Name = keyValue.Name,
                Value = keyValue.Value
            };
            _records.Add(record);
        }
        else
        {
            record.Value = keyValue.Value;
        }

        return Task.CompletedTask;
    }

    public Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken)
    {
        var record = _records.SingleOrDefault(r => r.Name == key);
        return Task.FromResult(record);
    }

    public Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_records);
    }
}
