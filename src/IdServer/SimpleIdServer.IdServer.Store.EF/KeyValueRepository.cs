// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Configuration;
using SimpleIdServer.Configuration.Models;

namespace SimpleIdServer.IdServer.Store.EF;

public class KeyValueRepository : IKeyValueRepository
{
    private readonly StoreDbContext _dbContext;

    public KeyValueRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ConfigurationKeyPairValueRecord keyValue)
        => _dbContext.ConfigurationKeyPairValueRecords.Add(keyValue);

    public void Update(ConfigurationKeyPairValueRecord keyValue)
    {
    }

    public Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken)
        => _dbContext.ConfigurationKeyPairValueRecords.SingleOrDefaultAsync(c => c.Name == key, cancellationToken);

    public Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken)
    {
        if (!_dbContext.Database.IsInMemory() && _dbContext.Database.GetPendingMigrations().Any()) return Task.FromResult(new List<ConfigurationKeyPairValueRecord>());
        return _dbContext.ConfigurationKeyPairValueRecords.ToListAsync(cancellationToken);
    }

    public async Task AddOrUpdate(ConfigurationKeyPairValueRecord keyValue, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ConfigurationKeyPairValueRecords.SingleOrDefaultAsync(c => c.Name == keyValue.Name, cancellationToken);
        if (record == null)
        {
            record = new ConfigurationKeyPairValueRecord
            {
                Name = keyValue.Name
            };
            _dbContext.ConfigurationKeyPairValueRecords.Add(record);
        }

        record.Value = keyValue.Value;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
