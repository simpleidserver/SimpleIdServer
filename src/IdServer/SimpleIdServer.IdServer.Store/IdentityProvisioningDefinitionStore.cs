// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IIdentityProvisioningDefinitionStore
    {
        IQueryable<IdentityProvisioningDefinition> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class IdentityProvisioningDefinitionStore : IIdentityProvisioningDefinitionStore
    {
        private readonly StoreDbContext _dbContext;

        public IdentityProvisioningDefinitionStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<IdentityProvisioningDefinition> Query() => _dbContext.IdentityProvisioningDefinitions;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
