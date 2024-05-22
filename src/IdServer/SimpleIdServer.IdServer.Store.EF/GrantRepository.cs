// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class GrantRepository : IGrantRepository
{
    private readonly StoreDbContext _dbContext;

    public GrantRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Consent> Get(string id, CancellationToken cancellation)
        => _dbContext.Grants
            .Include(g => g.Scopes).ThenInclude(g => g.AuthorizedResources)
            .Include(g => g.User)
            .SingleOrDefaultAsync(g => g.Id == id, cancellation);

    public void Remove(Consent consent) => _dbContext.Grants.Remove(consent);
}
