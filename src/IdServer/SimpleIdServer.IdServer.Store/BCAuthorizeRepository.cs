// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IBCAuthorizeRepository
    {
        IQueryable<BCAuthorize> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class BCAuthorizeRepository : IBCAuthorizeRepository
    {
        private readonly StoreDbContext _dbContext;

        public BCAuthorizeRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<BCAuthorize> Query() => _dbContext.BCAuthorizeLst;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
