// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.OpenidFederation.Store.EF.Stores;

public class FederationEntityStore : IFederationEntityStore
{
    private readonly OpenidFederationDbContext _dbContext;

    public FederationEntityStore(OpenidFederationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.FirstOrDefaultAsync(r => r.Sub == sub && r.Realm == realm && r.IsSubordinate == true, cancellationToken);
    }

    public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.Where(f => f.Realm == realm && f.IsSubordinate == true).ToListAsync(cancellationToken);
    }

    public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.Where(f => f.Realm == realm && f.IsSubordinate == false).ToListAsync(cancellationToken);
    }

    public void Add(FederationEntity federationEntity)
    {
        _dbContext.FederationEntities.Add(federationEntity);
    }

    public Task<FederationEntity?> GetByUrl(string realm, string url, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.SingleOrDefaultAsync(f => f.Sub == url && f.Realm == realm, cancellationToken);
    }

    public async Task<SearchResult<FederationEntity>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        IQueryable<FederationEntity> query = _dbContext.FederationEntities
            .Where(p => p.Realm == realm)
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.CreateDateTime);

        var nb = query.Count();
        var federationEntities = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<FederationEntity>
        {
            Count = nb,
            Content = federationEntities
        };
    }

    public Task<FederationEntity?> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public void Remove(FederationEntity federationEntity)
    {
        _dbContext.FederationEntities.Remove(federationEntity);
    }
}
