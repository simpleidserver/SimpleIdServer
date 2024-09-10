// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.ApplicationProvider.Models;
using SimpleIdServer.FastFed.ApplicationProvider.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Store.EF;

public class IdentityProviderFederationStore : IIdentityProviderFederationStore
{
    private readonly FastFedDbContext _dbContext;

    public IdentityProviderFederationStore(FastFedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(IdentityProviderFederation identityProviderFederation)
        => _dbContext.IdentityProviderFederations.Add(identityProviderFederation);

    public Task<IdentityProviderFederation> Get(string entityId, CancellationToken cancellationToken)
        => _dbContext.IdentityProviderFederations.SingleOrDefaultAsync(i => i.EntityId == entityId, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
