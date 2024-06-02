// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IApiResourceRepository
{
    Task<ApiResource> Get(string realm, string name, CancellationToken cancellationToken);
    Task<ApiResource> GetByName(string realm, string name, CancellationToken cancellationToken);
    Task<List<ApiResource>> GetByNames(string realm, List<string> names, CancellationToken cancellationToken);
    Task<List<ApiResource>> GetByNamesOrAudiences(string realm, List<string> names, List<string> audiences, CancellationToken cancellationToken);
    Task<List<ApiResource>> GetByScopes(List<string> scopes, CancellationToken cancellationToken);
    Task<SearchResult<ApiResource>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    void Add(ApiResource apiResource);
    void Delete(ApiResource apiResource);
}
