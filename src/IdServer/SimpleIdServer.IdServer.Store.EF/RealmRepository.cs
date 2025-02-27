// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class RealmRepository : IRealmRepository
{
    private readonly StoreDbContext _dbContext;

    public RealmRepository(StoreDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public Task<List<Realm>> GetAll(CancellationToken cancellationToken)
        => _dbContext.Realms.ToListAsync(cancellationToken);

    public Task<Realm> Get(string name, CancellationToken cancellationToken)
        => _dbContext.Realms.SingleOrDefaultAsync(r => r.Name == name, cancellationToken);

    public void Add(Realm realm) =>_dbContext.Realms.Add(realm);

    public void Remove(Realm realm) => _dbContext.Realms.Remove(realm);
}