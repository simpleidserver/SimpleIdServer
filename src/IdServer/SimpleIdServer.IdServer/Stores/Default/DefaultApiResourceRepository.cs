// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultApiResourceRepository : IApiResourceRepository
{
    private readonly List<ApiResource> _apiResources;

    public DefaultApiResourceRepository(List<ApiResource> apiResources)
    {
        _apiResources = apiResources;
    }

    public Task<ApiResource> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = _apiResources.SingleOrDefault(p => p.Realms.Any(r => r.Name == realm) && p.Id == id);
        return Task.FromResult(result);
    }

    public Task<ApiResource> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var result = _apiResources.SingleOrDefault(p => p.Realms.Any(r => r.Name == realm) && p.Name == name);
        return Task.FromResult(result);
    }

    public Task<List<ApiResource>> GetByNames(List<string> names, CancellationToken cancellationToken)
    {
        var result = _apiResources.Where(r => names.Contains(r.Name)).ToList();
        return Task.FromResult(result);
    }

    public Task<List<ApiResource>> GetByNames(string realm, List<string> names, CancellationToken cancellationToken)
    {
        var result = _apiResources.Where(r => names.Contains(r.Name) && r.Realms.Any(re => re.Name == realm)).ToList();
        return Task.FromResult(result);
    }

    public Task<List<ApiResource>> GetByNamesOrAudiences(string realm, List<string> names, List<string> audiences, CancellationToken cancellationToken)
    {
        var result = _apiResources.Where(r => (names.Contains(r.Name) || audiences.Contains(r.Audience)) && r.Realms.Any(rm => rm.Name == realm)).ToList();
        return Task.FromResult(result);
    }

    public Task<List<ApiResource>> GetByScopes(List<string> scopes, CancellationToken cancellationToken)
    {
        var result = _apiResources.Where(r => r.Scopes.Any(s => scopes.Contains(s.Name))).ToList();
        return Task.FromResult(result);
    }

    public Task<SearchResult<ApiResource>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        // On utilise AsQueryable pour permettre l'utilisation de Dynamic LINQ.
        var query = _apiResources.AsQueryable().Where(p => p.Realms.Any(r => r.Name == realm));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderBy(r => r.Name);

        var nb = query.Count();
        var apiResources = query.Skip(request.Skip.HasValue ? request.Skip.Value : 0)
                                .Take(request.Take.HasValue ? request.Take.Value : nb)
                                .ToList();

        return Task.FromResult(new SearchResult<ApiResource>
        {
            Count = nb,
            Content = apiResources
        });
    }

    public void Add(ApiResource apiResource)
        => _apiResources.Add(apiResource);

    public void Delete(ApiResource apiResource)
        => _apiResources.Remove(apiResource);

    public Task<List<ApiResource>> GetByIds(List<string> ids, CancellationToken cancellationToken)
    {
        return Task.FromResult(_apiResources.Where(r => ids.Contains(r.Id)).ToList());
    }

    public void Update(ApiResource apiResource)
    {
    }

    public Task BulkAdd(List<ApiResource> apiResources)
    {
        _apiResources.AddRange(apiResources);
        return Task.CompletedTask;
    }
}
