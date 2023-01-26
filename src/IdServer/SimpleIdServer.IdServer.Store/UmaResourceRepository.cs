// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUmaResourceRepository
    {
        IQueryable<UMAResource> Query();
        void Add(UMAResource resource);
        void Delete(UMAResource resource);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class UmaResourceRepository : IUmaResourceRepository
    {
        private readonly StoreDbContext _dbContext;

        public UmaResourceRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UMAResource> Query() => _dbContext.UmaResources;

        public void Add(UMAResource resource) => _dbContext.UmaResources.Add(resource);

        public void Delete(UMAResource resource) => _dbContext.UmaResources.Remove(resource);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
