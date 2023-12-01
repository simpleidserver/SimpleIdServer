// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IApiResourceRepository
    {
        IQueryable<ApiResource> Query();
        void Add(ApiResource apiResource);
        void Delete(ApiResource apiResource);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly StoreDbContext _dbContext;

        public ApiResourceRepository(StoreDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public void Add(ApiResource apiResource) => _dbContext.ApiResources.Add(apiResource);

        public void Delete(ApiResource apiResource) => _dbContext.ApiResources.Remove(apiResource);

        public IQueryable<ApiResource> Query() => _dbContext.ApiResources;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
