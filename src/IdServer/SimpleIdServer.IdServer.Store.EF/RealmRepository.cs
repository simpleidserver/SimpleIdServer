// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

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

    public async Task<SearchResult<RealmRole>> SearchRoles(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        IQueryable<RealmRole> query = _dbContext.RealmRoles
            .Where(p => p.RealmName == realm)
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.UpdateDateTime);

        var nb = query.Count();
        var clients = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<RealmRole>
        {
            Count = nb,
            Content = clients
        };
    }

    public Task<RealmRole> GetRole(string id, CancellationToken cancellationToken)
    {
        return _dbContext.RealmRoles.Include(r => r.Scopes).ThenInclude(s => s.Scope).SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public void DeleteRole(RealmRole role)
    {
        _dbContext.RealmRoles.Remove(role);
    }
}