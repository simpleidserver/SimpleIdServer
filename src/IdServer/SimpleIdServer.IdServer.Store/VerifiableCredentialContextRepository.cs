// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IVerifiableCredentialContextRepository
    {
        IQueryable<VerifiableCredentialContext> Query();
    }

    public class VerifiableCredentialContextRepository : IVerifiableCredentialContextRepository
    {
        private readonly StoreDbContext _dbContext;

        public VerifiableCredentialContextRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<VerifiableCredentialContext> Query() => _dbContext.VerifiableCredentialContexts;
    }
}
