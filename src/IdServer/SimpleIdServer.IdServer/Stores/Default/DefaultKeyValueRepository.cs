// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultKeyValueRepository : IKeyValueRepository
{
    private readonly List<ConfigurationKeyPairValueRecord> _keyValues;

    public DefaultKeyValueRepository(List<ConfigurationKeyPairValueRecord> keyValues)
    {
        _keyValues = keyValues;
    }

    public void Add(ConfigurationKeyPairValueRecord keyValue)
        => _keyValues.Add(keyValue);

    public void Update(ConfigurationKeyPairValueRecord keyValue)
    {
    }

    public Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken)
        => Task.FromResult(_keyValues.SingleOrDefault(c => c.Name == key));

    public Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_keyValues);
    }
}
