// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class RealmRepository : IRealmRepository
{
    private readonly DbContext _dbContext;
    
    public RealmRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Realm>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRealm>().ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Realm> Get(string name, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRealm>()
            .FirstAsync(r => r.RealmsName == name, cancellationToken);
        return result?.ToDomain();
    }

    public void Add(Realm realm)
        => _dbContext.Client.Insertable(Transform(realm)).ExecuteCommand();

    private static SugarRealm Transform(Realm realm)
    {
        return new SugarRealm
        {
            CreateDateTime = realm.CreateDateTime,
            UpdateDateTime = realm.UpdateDateTime,
            Description = realm.Description,
            RealmsName = realm.Name
        };
    }

    public Task<SearchResult<RealmRole>> SearchRoles(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<RealmRole> GetRole(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void DeleteRole(RealmRole role)
    {
        throw new NotImplementedException();
    }
}