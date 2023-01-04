// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuthenticationSchemeProviderRepository
    {
        IQueryable<AuthenticationSchemeProvider> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuthenticationSchemeProviderRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AuthenticationSchemeProvider> Query()
        {
            return _dbContext.AuthenticationSchemeProviders;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
