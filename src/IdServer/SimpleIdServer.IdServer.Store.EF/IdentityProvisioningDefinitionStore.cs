// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

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
