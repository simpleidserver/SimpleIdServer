// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class ClaimProviderRepository : IClaimProviderRepository
{
    private readonly DbContext _dbContext;

    public ClaimProviderRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ClaimProvider>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarClaimProvider>()
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }
}
