// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IScopeRepository
{
    IQueryable<Scope> Query();
    Task<Scope> GetByName(string realm, string scopeName, CancellationToken cancellationToken);
    Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken);
    Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken);
    void Add(Scope scope);
    void DeleteRange(IEnumerable<Scope> scopes);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
