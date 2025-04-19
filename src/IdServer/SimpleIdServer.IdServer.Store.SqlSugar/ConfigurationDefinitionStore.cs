// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit.Initializers;
using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class ConfigurationDefinitionStore : IConfigurationDefinitionStore
{
    private readonly DbContext _dbContext;

    public ConfigurationDefinitionStore(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConfigurationDefinition> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarConfigurationDefinition>()
            .Includes(c => c.ConfigurationDefinitionRecords, r => r.Values, r => r.Translations)
            .Includes(c => c.ConfigurationDefinitionRecords, r => r.Translations)
            .SingleAsync(c => c.Id == id);
        return result?.ToDomain();
    }

    public void Add(ConfigurationDefinition cnfigurationDefinition)
    {
        _dbContext.Client.InsertNav(SugarConfigurationDefinition.Transform(cnfigurationDefinition))
            .Include(c => c.ConfigurationDefinitionRecords).ThenInclude(c => c.Values).ThenInclude(c => c.Translations)
            .Include(c => c.ConfigurationDefinitionRecords).ThenInclude(c => c.Translations)
            .ExecuteCommand();
    }

    public async Task<List<ConfigurationDefinition>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarConfigurationDefinition>()
            .Includes(c => c.ConfigurationDefinitionRecords, r => r.Values, r => r.Translations)
            .Includes(c => c.ConfigurationDefinitionRecords, r => r.Translations)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }
}
