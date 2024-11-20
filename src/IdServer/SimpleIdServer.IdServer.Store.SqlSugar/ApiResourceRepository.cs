// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class ApiResourceRepository : IApiResourceRepository
{
    private readonly DbContext _dbContext;

    public ApiResourceRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResource> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarApiResource>()
            .Includes(a => a.Realms)
            .Includes(a => a.Scopes)
            .FirstAsync(a => a.Realms.Any(r => r.RealmsName == realm) && a.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<ApiResource> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarApiResource>()
            .Includes(a => a.Realms)
            .Includes(a => a.Scopes)
            .FirstAsync(a => a.Realms.Any(r => r.RealmsName == realm) && a.Name == name, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<ApiResource>> GetByNames(string realm, List<string> names, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarApiResource>()
            .Includes(r => r.Realms)
            .Where(r => names.Contains(r.Name) && r.Realms.Any(re => re.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<ApiResource>> GetByNamesOrAudiences(string realm, List<string> names, List<string> audiences, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarApiResource>()
            .Includes(r => r.Realms)
            .Includes(r => r.Scopes)
            .Where(r => (names.Contains(r.Name) || audiences.Contains(r.Audience)) && r.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<ApiResource>> GetByScopes(List<string> scopes, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarApiResource>()
            .Includes(r => r.Realms)
            .Includes(r => r.Scopes)
            .Where(r => r.Scopes.Any(s => scopes.Contains(s.Name)))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<SearchResult<ApiResource>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarApiResource>()
                .Includes(p => p.Realms)
                .Includes(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm));
        query = query.OrderByDescending(a => a.UpdateDateTime);
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderBy(r => r.Name);
        */

        var nb = query.Count();

        var apiResources = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<ApiResource>
        {
            Count = nb,
            Content = apiResources.Select(r => r.ToDomain()).ToList()
        };
    }

    public void Add(ApiResource apiResource)
    {
        var res = new SugarApiResource
        {
            Audience = apiResource.Audience,
            CreateDateTime = apiResource.CreateDateTime,
            Description = apiResource.Description,
            Id = apiResource.Id,
            Name = apiResource.Name,
            UpdateDateTime = apiResource.UpdateDateTime,
            Realms = apiResource.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList()
        };
        _dbContext.Client.InsertNav(res)
            .Include(r => r.Realms)
            .ExecuteCommand();
    }

    public void Delete(ApiResource apiResource)
    {
        _dbContext.Client.Deleteable(apiResource).ExecuteCommand();
    }
}