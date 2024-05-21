// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class AuthenticationSchemeProviderDefinitionRepository : IAuthenticationSchemeProviderDefinitionRepository
{
    private readonly DbContext _dbContext;

    public AuthenticationSchemeProviderDefinitionRepository(DbContext dbContext)
    {
        _dbContext = dbContext;    
    }

    public async Task<AuthenticationSchemeProviderDefinition> Get(string name, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuthenticationSchemeProviderDefinition>()
            .FirstAsync(s => s.Name == name, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<AuthenticationSchemeProviderDefinition>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuthenticationSchemeProviderDefinition>()
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }
}
