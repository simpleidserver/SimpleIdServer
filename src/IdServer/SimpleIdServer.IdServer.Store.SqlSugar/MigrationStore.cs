// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class MigrationStore : IMigrationStore
{
    public void Add(MigrationExecution migrationExecution)
    {
        throw new NotImplementedException();
    }

    public Task<MigrationExecution> Get(string realm, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<MigrationExecution>> GetAll(string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
