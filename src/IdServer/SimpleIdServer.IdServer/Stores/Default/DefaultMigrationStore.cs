// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultMigrationStore : IMigrationStore
{
    private readonly List<MigrationExecution> _migrations = new List<MigrationExecution>();

    public DefaultMigrationStore()
    {
        
    }

    public void Add(MigrationExecution migrationExecution)
    {
        _migrations.Add(migrationExecution);
    }

    public Task<MigrationExecution> Get(string realm, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_migrations.SingleOrDefault(m => m.Realm == realm && m.Name == name));
    }

    public Task<List<MigrationExecution>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_migrations.Where(m => m.Realm == realm).ToList());
    }
}
