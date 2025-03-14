// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.OpenidFederation.Domains;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.OpenidFederation.Stores;

public class DefaultFederationEntityStore : IFederationEntityStore
{
    private readonly List<FederationEntity> _entities;

    public DefaultFederationEntityStore(List<FederationEntity> entities)
    {
        _entities = entities;
    }

    public void Add(FederationEntity federationEntity)
    {
        _entities.Add(federationEntity);
    }

    public Task<FederationEntity?> Get(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_entities.SingleOrDefault(e => e.Id == id));
    }

    public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_entities.Where(f => f.Realm == realm && f.IsSubordinate == true).ToList());
    }

    public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_entities.Where(f => f.Realm == realm && f.IsSubordinate == false).ToList());
    }

    public Task<FederationEntity?> GetByUrl(string realm, string url, CancellationToken cancellationToken)
    {
        return Task.FromResult(_entities.SingleOrDefault(f => f.Sub == url && f.Realm == realm));
    }

    public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_entities.SingleOrDefault(r => r.Sub == sub && r.Realm == realm && r.IsSubordinate == true));
    }

    public void Remove(FederationEntity federationEntity)
    {
        _entities.Remove(federationEntity);
    }

    public Task<SearchResult<FederationEntity>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
       var query = _entities
            .AsQueryable()
            .Where(p => p.Realm == realm);
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.CreateDateTime);

        var nb = query.Count();
        var federationEntities = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return Task.FromResult(new SearchResult<FederationEntity>
        {
            Count = nb,
            Content = federationEntities
        });
    }
}
