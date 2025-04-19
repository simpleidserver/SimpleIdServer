// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class ConfigurationDefinitionStore : IConfigurationDefinitionStore
{
    private readonly StoreDbContext _dbContext;

    public ConfigurationDefinitionStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;

    }

    public Task<ConfigurationDefinition> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.Definitions.Include(c => c.Records).ThenInclude(r => r.Values).ThenInclude(r => r.Translations)
            .Include(c => c.Records).ThenInclude(r => r.Translations)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public void Add(ConfigurationDefinition configurationDefinition)
    {
        _dbContext.Definitions.Add(configurationDefinition);
    }

    public Task<List<ConfigurationDefinition>> GetAll(CancellationToken cancellationToken)
        => _dbContext.Definitions.Include(c => c.Records).ThenInclude(r => r.Values).ThenInclude(r => r.Translations)
            .Include(c => c.Records).ThenInclude(r => r.Translations)
            .ToListAsync(cancellationToken);
}