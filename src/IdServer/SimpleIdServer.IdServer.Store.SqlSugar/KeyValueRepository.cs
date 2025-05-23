﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;
using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;

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
            if (keyValue == null) return;
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

        private static SugarConfigurationKeyPairValueRecord Transform(ConfigurationKeyPairValueRecord keyValue)
        {
            return new SugarConfigurationKeyPairValueRecord
            {
                Name = keyValue.Name,
                Value = keyValue.Value,
            };
        }

        public async Task AddOrUpdate(ConfigurationKeyPairValueRecord keyValue, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarConfigurationKeyPairValueRecord>()
                .FirstAsync(c => c.Name == keyValue.Name, cancellationToken);
            if (result == null)
            {
                result = new SugarConfigurationKeyPairValueRecord
                {
                    Name = keyValue.Name,
                    Value = keyValue.Value
                };
                _dbContext.Client.Insertable(Transform(keyValue)).ExecuteCommand();
            }
            else
            {
                _dbContext.Client.Updateable(Transform(keyValue)).ExecuteCommand();
            }
        }
    }
}
