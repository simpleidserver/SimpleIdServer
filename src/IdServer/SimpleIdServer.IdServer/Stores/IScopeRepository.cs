// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IScopeRepository
{
    Task<List<Scope>> Get(List<string> ids, CancellationToken cancellationToken);
    Task<Scope> Get(string realm, string id, CancellationToken cancellationToken);
    Task<Scope> GetByName(string realm, string scopeName, CancellationToken cancellationToken);
    Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken);
    Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken);
    Task<List<Scope>> GetAll(string realm, List<string> scopeNames, CancellationToken cancellationToken);
    Task<List<Scope>> GetByNames(List<string> scopeNames, CancellationToken cancellation);
    Task<List<Scope>> GetAllRealmScopes(string realm, CancellationToken cancellationToken);
    Task<SearchResult<Scope>> Search(string realm, SearchScopeRequest request, CancellationToken cancellationToken);
    void Add(Scope scope);
    void Update(Scope scope);
    void DeleteRange(IEnumerable<Scope> scopes);
}