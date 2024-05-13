// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IKeyValueRepository
{
    Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken);
    Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken);
    void Add(ConfigurationKeyPairValueRecord keyValue);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
