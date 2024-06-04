// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class AuthenticationSchemeProviderDefinitionRepository : IAuthenticationSchemeProviderDefinitionRepository
{
    private readonly StoreDbContext _dbContext;

    public AuthenticationSchemeProviderDefinitionRepository(StoreDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public void Add(AuthenticationSchemeProviderDefinition definition)
    {
        _dbContext.AuthenticationSchemeProviderDefinitions.Add(definition);
    }

    public Task<AuthenticationSchemeProviderDefinition> Get(string name, CancellationToken cancellationToken)
    {
        return _dbContext.AuthenticationSchemeProviderDefinitions.SingleOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public Task<List<AuthenticationSchemeProviderDefinition>> GetAll(CancellationToken cancellationToken)
    {
        return _dbContext.AuthenticationSchemeProviderDefinitions.AsNoTracking().ToListAsync(cancellationToken);
    }
}
