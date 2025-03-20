// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class IdentityProvisioningStore : IIdentityProvisioningStore
{
    private readonly StoreDbContext _dbContext;

    public IdentityProvisioningStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchResult<IdentityProvisioning>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.IdentityProvisioningLst
            .Include(p => p.Realms)
            .Where(p => p.Realms.Any(r => r.Name == realm))
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var idProviders = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<IdentityProvisioning>
        {
            Count = nb,
            Content = idProviders
        };
    }

    public Task<IdentityProvisioning> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.IdentityProvisioningLst
                    .Include(p => p.Realms)
                    .Include(p => p.Histories)
                    .Include(p => p.Definition).ThenInclude(d => d.MappingRules)
                    .SingleOrDefaultAsync(p => p.Realms.Any(r => r.Name == realm) && p.Id == id, cancellationToken);
    }

    public void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst) => _dbContext.IdentityProvisioningLst.RemoveRange(identityProvisioningLst);

    public void Remove(IdentityProvisioning identityProvisioning) => _dbContext.IdentityProvisioningLst.Remove(identityProvisioning);

    public void Update(IdentityProvisioning identityProvisioning)
    {

    }

    public void Add(IdentityProvisioningDefinition identityProvisioningDefinition)
    {
        _dbContext.IdentityProvisioningDefinitions.Add(identityProvisioningDefinition);
    }

    public void Update(IdentityProvisioningDefinition identityProvisioningDefinition)
    {

    }

    public Task<IdentityProvisioningDefinition> GetDefinitionByName(string name, CancellationToken cancellationToken)
    {
        return _dbContext.IdentityProvisioningDefinitions
                    .SingleOrDefaultAsync(p => p.Name == name, cancellationToken);
    }
}
