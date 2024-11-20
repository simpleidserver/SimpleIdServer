// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class ScopeRepository : IScopeRepository
{
    private readonly StoreDbContext _dbContext;

    public ScopeRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Scope>> Get(List<string> ids, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .Include(s => s.ClaimMappers)
                .Where(s => ids.Contains(s.Id))
                .ToListAsync(cancellationToken);
    }

    public Task<Scope> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(p => p.Realms)
                .Include(p => p.ClaimMappers)
                .Include(p => p.ApiResources)
                .FirstOrDefaultAsync(p => p.Realms.Any(r => r.Name == realm) && p.Id == id, cancellationToken);
    }

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
                .Include(s => s.ClaimMappers)
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

    public Task<List<Scope>> GetAll(string realm, List<string> scopeNames, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
    }

    public Task<List<Scope>> GetAllRealmScopes(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.Scopes
                .Include(s => s.Realms)
                .Where(s => s.Component != null && s.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
    }

    public async Task<SearchResult<Scope>> Search(string realm, SearchScopeRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Scopes
                .Include(p => p.Realms)
                .Include(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.Name == realm) && ((request.IsRole && p.Type == ScopeTypes.ROLE) || (!request.IsRole && (p.Type == ScopeTypes.IDENTITY || p.Type == ScopeTypes.APIRESOURCE))))
                .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(s => s.UpdateDateTime);

        if (request.Protocols != null && request.Protocols.Any())
            query = query.Where(q => request.Protocols.Contains(q.Protocol));
        var nb = query.Count();
        var scopes = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<Scope>
        {
            Count = nb,
            Content = scopes
        };
    }

    public void DeleteRange(IEnumerable<Scope> scopes) => _dbContext.Scopes.RemoveRange(scopes);

    public void Add(Scope scope) => _dbContext.Scopes.Add(scope);

    public void Update(Scope scope) { }
}
