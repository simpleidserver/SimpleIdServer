// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class BCAuthorizeRepository : IBCAuthorizeRepository
{
    private readonly StoreDbContext _dbContext;

    public BCAuthorizeRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BCAuthorize> GetById(string id, CancellationToken cancellationToken)
    {
        return _dbContext.BCAuthorizeLst
            .Include(a => a.Histories)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public void Add(BCAuthorize bcAuthorize) => _dbContext.BCAuthorizeLst.Add(bcAuthorize);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
