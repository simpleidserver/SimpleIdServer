// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuthenticationSchemeProviderRepository
    {
        IQueryable<AuthenticationSchemeProvider> Query();
        void Remove(AuthenticationSchemeProvider idProvider);
        void Add(AuthenticationSchemeProvider idProvider);
        void RemoveRange(IEnumerable<AuthenticationSchemeProvider> idProviders);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuthenticationSchemeProviderRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AuthenticationSchemeProvider> Query() => _dbContext.AuthenticationSchemeProviders;

        public void Remove(AuthenticationSchemeProvider idProvider) => _dbContext.AuthenticationSchemeProviders.Remove(idProvider);

        public void Add(AuthenticationSchemeProvider idProvider) => _dbContext.AuthenticationSchemeProviders.Add(idProvider);

        public void RemoveRange(IEnumerable<AuthenticationSchemeProvider> idProviders) => _dbContext.AuthenticationSchemeProviders.RemoveRange(idProviders);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
