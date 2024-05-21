// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class ClaimProviderRepository : IClaimProviderRepository
{
    private readonly StoreDbContext _dbContext;

    public ClaimProviderRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ClaimProvider>> GetAll(CancellationToken cancellationToken)
        => _dbContext.ClaimProviders.ToListAsync(cancellationToken);
}
