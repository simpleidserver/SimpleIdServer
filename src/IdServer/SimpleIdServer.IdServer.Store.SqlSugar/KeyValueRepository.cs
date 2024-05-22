// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class KeyValueRepository : IKeyValueRepository
    {
        private readonly DbContext _dbContext;

        public KeyValueRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ConfigurationKeyPairValueRecord keyValue)
        {
            _dbContext.Client.Insertable(Transform(keyValue)).ExecuteCommand();
        }

        public void Update(ConfigurationKeyPairValueRecord keyValue)
        {
            _dbContext.Client.Updateable(Transform(keyValue)).ExecuteCommand();
        }

        public async Task<ConfigurationKeyPairValueRecord> Get(string key, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarConfigurationKeyPairValueRecord>()
                .FirstAsync(c => c.Name == key, cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<ConfigurationKeyPairValueRecord>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarConfigurationKeyPairValueRecord>()
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static SugarConfigurationKeyPairValueRecord Transform(ConfigurationKeyPairValueRecord keyValue)
        {
            return new SugarConfigurationKeyPairValueRecord
            {
                Name = keyValue.Name,
                Value = keyValue.Value,
            };
        }
    }
}
