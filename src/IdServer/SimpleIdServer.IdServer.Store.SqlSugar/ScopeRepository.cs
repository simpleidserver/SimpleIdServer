// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class ScopeRepository : IScopeRepository
    {
        public void Add(Scope scope)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(IEnumerable<Scope> scopes)
        {
            throw new NotImplementedException();
        }

        public Task<Scope> Get(string realm, string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Scope>> GetAll(string realm, List<string> scopeNames, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Scope> GetByName(string realm, string scopeName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Scope> Query()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResult<Scope>> Search(string realm, SearchScopeRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
