// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IGrantRepository
    {
        IQueryable<Consent> Query();
        void Remove(Consent consent);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class GrantRepository : IGrantRepository
    {
        private readonly StoreDbContext _dbContext;

        public GrantRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Consent> Query() => _dbContext.Grants;

        public void Remove(Consent consent) => _dbContext.Grants.Remove(consent);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
