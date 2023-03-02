// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuthenticationSchemeProviderDefinitionRepository
    {
        IQueryable<AuthenticationSchemeProviderDefinition> Query();
    }

    public class AuthenticationSchemeProviderDefinitionRepository : IAuthenticationSchemeProviderDefinitionRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuthenticationSchemeProviderDefinitionRepository(StoreDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public IQueryable<AuthenticationSchemeProviderDefinition> Query() => _dbContext.AuthenticationSchemeProviderDefinitions;
    }
}
