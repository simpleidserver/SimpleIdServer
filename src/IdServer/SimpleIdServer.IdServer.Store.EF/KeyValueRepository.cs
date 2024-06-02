// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF
{
    internal class KeyValueRepository : IKeyValueRepository
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
            => _dbContext.ConfigurationKeyPairValueRecords.ToListAsync(cancellationToken);

        public Task<int> SaveChanges(CancellationToken cancellationToken)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
