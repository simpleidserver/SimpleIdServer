// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration;

public interface IKeyValueRepository
{
    Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken);
    Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken);
    Task AddOrUpdate(ConfigurationKeyPairValueRecord keyValue, CancellationToken cancellationToken);
}