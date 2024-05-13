// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class SerializedFileKeyStore : IFileSerializedKeyStore
{
    private readonly StoreDbContext _dbContext;

    public SerializedFileKeyStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IQueryable<SerializedFileKey> Query() => _dbContext.SerializedFileKeys;

    public Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.SerializedFileKeys
            .Include(s => s.Realms)
            .Where(s => s.Realms.Any(r => r.Name == realm))
            .ToListAsync(cancellationToken);
    }

    public void Add(SerializedFileKey key) => _dbContext.SerializedFileKeys.Add(key);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
