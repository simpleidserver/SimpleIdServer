// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class GrantRepository : IGrantRepository
{
    private readonly DbContext _dbContext;

    public GrantRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Consent> Get(string id, CancellationToken cancellation)
    {
        var result = await _dbContext.Client.Queryable<SugarConsent>()
            .Includes(g => g.Scopes, g => g.AuthorizedResources)
            .Includes(g => g.User)
            .FirstAsync(g => g.Id == id, cancellation);
        return result?.ToDomain();
    }

    public void Remove(Consent consent)
    {
        _dbContext.Client.Deleteable(Transform(consent)).ExecuteCommand();
    }

    private static SugarConsent Transform(Consent consent)
    {
        return new SugarConsent
        {
            Id = consent.Id
        };
    }
}
