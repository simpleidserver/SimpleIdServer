// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IGrantRepository
    {
        IQueryable<Grant> Query();
        void Add(Grant grant);
        void Update(Grant grant);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class GrantRepository : IGrantRepository
    {
        private readonly StoreDbContext _dbContext;

        public GrantRepository(StoreDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public void Add(Grant grant) => _dbContext.Grants.Add(grant);

        public IQueryable<Grant> Query() => _dbContext.Grants;

        public void Update(Grant grant)
        {
            _dbContext.Grants.Update(grant);
            foreach (var scope in grant.Scopes)
                _dbContext.Update(scope);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
