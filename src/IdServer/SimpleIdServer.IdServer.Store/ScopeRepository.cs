// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IScopeRepository
    {
        IQueryable<Scope> Query();
        void Add(Scope scope);
        void DeleteRange(IEnumerable<Scope> scopes);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class ScopeRepository : IScopeRepository
    {
        private readonly StoreDbContext _dbContext;

        public ScopeRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Scope> Query() => _dbContext.Scopes;

        public void DeleteRange(IEnumerable<Scope> scopes) => _dbContext.Scopes.RemoveRange(scopes);

        public void Add(Scope scope) => _dbContext.Scopes.Add(scope);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
