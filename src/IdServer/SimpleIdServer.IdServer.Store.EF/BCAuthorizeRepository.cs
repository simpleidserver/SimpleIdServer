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
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<List<BCAuthorize>> GetAllConfirmed(List<string> notificationModes, CancellationToken cancellationToken)
    {
        return _dbContext.BCAuthorizeLst
            .Include(a => a.Histories)
            .Where(a => a.LastStatus == Domains.BCAuthorizeStatus.Confirmed && notificationModes.Contains(a.NotificationMode) && DateTime.UtcNow < a.ExpirationDateTime)
            .ToListAsync(cancellationToken);
    }

    public void Add(BCAuthorize bcAuthorize) => _dbContext.BCAuthorizeLst.Add(bcAuthorize);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
