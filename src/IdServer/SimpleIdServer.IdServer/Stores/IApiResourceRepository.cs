// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IApiResourceRepository
{
    Task<ApiResource> Get(string realm, string name, CancellationToken cancellationToken);
    Task<ApiResource> GetByName(string realm, string name, CancellationToken cancellationToken);
    Task<SearchResult<ApiResource>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    IQueryable<ApiResource> Query();
    void Add(ApiResource apiResource);
    void Delete(ApiResource apiResource);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
