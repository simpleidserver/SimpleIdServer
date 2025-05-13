// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class MigrationStore : IMigrationStore
{
    private readonly StoreDbContext _storeDbContext;

    public MigrationStore(StoreDbContext dbcontext)
    {
        _storeDbContext = dbcontext;
    }

    public void Add(MigrationExecution migrationExecution)
    {
        _storeDbContext.MigrationExecutions.Add(migrationExecution);
    }

    public Task<MigrationExecution> Get(string realm, string name, CancellationToken cancellationToken)
    {
        return _storeDbContext.MigrationExecutions
            .Include(e => e.Histories)
            .SingleOrDefaultAsync(e => e.Realm == realm && e.Name == name, cancellationToken);
    }

    public Task<List<MigrationExecution>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return _storeDbContext.MigrationExecutions
            .Include(e => e.Histories)
            .Where(e => e.Realm == realm)
            .ToListAsync(cancellationToken);
    }
}
