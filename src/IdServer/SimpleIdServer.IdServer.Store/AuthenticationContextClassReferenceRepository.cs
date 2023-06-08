// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuthenticationContextClassReferenceRepository
    {
        IQueryable<AuthenticationContextClassReference> Query();
        void Add(AuthenticationContextClassReference record);
        void Delete(AuthenticationContextClassReference record);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuthenticationContextClassReferenceRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AuthenticationContextClassReference> Query() => _dbContext.Acrs;

        public void Add(AuthenticationContextClassReference record) => _dbContext.Acrs.Add(record);

        public void Delete(AuthenticationContextClassReference record) => _dbContext.Acrs.Remove(record);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
