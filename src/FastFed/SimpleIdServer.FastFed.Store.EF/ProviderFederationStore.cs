// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Store.EF;

public class ProviderFederationStore : IProviderFederationStore
{
    private readonly FastFedDbContext _dbContext;

    public ProviderFederationStore(FastFedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(IdentityProviderFederation identityProviderFederation)
        => _dbContext.IdentityProviderFederations.Add(identityProviderFederation);

    public Task<IdentityProviderFederation> Get(string entityId, CancellationToken cancellationToken)
        => _dbContext.IdentityProviderFederations.Include(i => i.Capabilities).SingleOrDefaultAsync(i => i.EntityId == entityId, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
