// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IMigrationStore
{
    Task<List<MigrationExecution>> GetAll(string realm, CancellationToken cancellationToken);
    Task<MigrationExecution> Get(string realm, string name, CancellationToken cancellationToken);
    void Add(MigrationExecution migrationExecution);
}
