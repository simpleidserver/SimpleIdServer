// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IScopeRepository
    {
        IQueryable<Scope> Query();
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

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
