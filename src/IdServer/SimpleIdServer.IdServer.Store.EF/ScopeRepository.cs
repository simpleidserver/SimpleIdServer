// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class ScopeRepository : IScopeRepository
{
    private readonly StoreDbContext _dbContext;

    public ScopeRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<Scope> Query() => _dbContext.Scopes;

    public Task<Scope> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .SingleOrDefaultAsync(s => s.Name == name && s.Realms.Any(r => r.Name == realm), cancellationToken);
    }

    public Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
    }

    public Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .Include(s => s.ClaimMappers)
                .Where(s => s.IsExposedInConfigurationEdp && s.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
    }

    public void DeleteRange(IEnumerable<Scope> scopes) => _dbContext.Scopes.RemoveRange(scopes);

    public void Add(Scope scope) => _dbContext.Scopes.Add(scope);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
