// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class UmaResourceRepository : IUmaResourceRepository
{
    private readonly StoreDbContext _dbContext;

    public UmaResourceRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UMAResource> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.UmaResources
            .Include(r => r.Translations)
            .Include(r => r.Permissions).ThenInclude(p => p.Claims)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<List<UMAResource>> GetByIds(List<string> resourceIds, CancellationToken cancellationToken)
    {
        return _dbContext.UmaResources
            .Include(r => r.Permissions).ThenInclude(p => p.Claims)
            .Where(r => resourceIds.Contains(r.Id)).ToListAsync(cancellationToken);
    }

    public Task<List<UMAResource>> GetAll(CancellationToken cancellationToken)
    {
        return _dbContext.UmaResources.ToListAsync(cancellationToken);
    }

    public void Add(UMAResource resource) => _dbContext.UmaResources.Add(resource);

    public void Delete(UMAResource resource) => _dbContext.UmaResources.Remove(resource);

    public void Update(UMAResource resource)
    {

    }
}
