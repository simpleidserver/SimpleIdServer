// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
{
    private readonly StoreDbContext _dbContext;

    public AuthenticationSchemeProviderRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(AuthenticationSchemeProvider idProvider) => _dbContext.AuthenticationSchemeProviders.Add(idProvider);

    public Task<AuthenticationSchemeProvider> Get(string realm, string name, CancellationToken cancellationToken)
    {
        return _dbContext.AuthenticationSchemeProviders
                .Include(p => p.Realms)
                .Include(p => p.AuthSchemeProviderDefinition)
                .Include(p => p.Mappers)
                .Where(p => p.Realms.Any(r => r.Name == realm))
                .SingleOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<SearchResult<AuthenticationSchemeProvider>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.AuthenticationSchemeProviders
                .Include(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.Name == realm))
                .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var idProviders = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<AuthenticationSchemeProvider>
        {
            Count = nb,
            Content = idProviders
        };
    }

    public void Remove(AuthenticationSchemeProvider idProvider) => _dbContext.AuthenticationSchemeProviders.Remove(idProvider);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
