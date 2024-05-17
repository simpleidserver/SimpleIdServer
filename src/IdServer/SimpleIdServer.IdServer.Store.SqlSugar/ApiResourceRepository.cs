// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
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

    public void Add(ApiResource apiResource)
    {
        throw new NotImplementedException();
    }

    public void Delete(ApiResource apiResource)
    {
        throw new NotImplementedException();
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

    public Task<List<ApiResource>> GetByNamesOrAudiences(string realm, List<string> names, List<string> audiences, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<ApiResource>> GetByScopes(List<string> scopes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<ApiResource> Query()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchResult<ApiResource>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

[SugarTable("tb_a1")]
public class ClassA
{
    [SugarColumn(IsPrimaryKey = true)]
    public string AId { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(ClassB.AId))]
    public List<ClassB> B { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}

[SugarTable("tb_b2")]
public class ClassB
{
    [SugarColumn(IsPrimaryKey = true)]
    public string BId { get; set; }
    public string AId { get; set; }
}