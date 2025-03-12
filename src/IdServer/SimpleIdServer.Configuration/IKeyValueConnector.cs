// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration;

public interface IKeyValueConnector
{
    Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken);
    Task Set(string key, string value, CancellationToken cancellationToken);
}

public class DefaultKeyValueConnector : IKeyValueConnector
{
    private readonly IKeyValueRepository _repository;

    public DefaultKeyValueConnector(IKeyValueRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _repository.GetAll(cancellationToken);
        return result.ToDictionary(r => r.Name, r => r.Value);
    }

    public Task Set(string key, string value, CancellationToken cancellationToken)
    {
        return _repository.AddOrUpdate(new Models.ConfigurationKeyPairValueRecord { Name = key, Value = value }, cancellationToken);
    }
}