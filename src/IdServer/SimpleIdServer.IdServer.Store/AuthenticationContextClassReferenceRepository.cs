// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuthenticationContextClassReferenceRepository
    {
        IQueryable<AuthenticationContextClassReference> Query();
    }

    public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuthenticationContextClassReferenceRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AuthenticationContextClassReference> Query() => _dbContext.Acrs;
    }
}
