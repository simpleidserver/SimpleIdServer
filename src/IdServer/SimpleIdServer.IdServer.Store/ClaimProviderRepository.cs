// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IClaimProviderRepository
    {
        IQueryable<ClaimProvider> Query();
    }

    public class ClaimProviderRepository : IClaimProviderRepository
    {
        private readonly StoreDbContext _dbContext;

        public ClaimProviderRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ClaimProvider> Query() => _dbContext.ClaimProviders;
    }
}
