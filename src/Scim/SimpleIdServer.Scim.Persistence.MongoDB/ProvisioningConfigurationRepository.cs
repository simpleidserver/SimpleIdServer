// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class ProvisioningConfigurationRepository : IProvisioningConfigurationRepository
    {
        private readonly SCIMDbContext _dbContext;
        private readonly IMongoClient _mongoClient;
        private readonly MongoDbOptions _options;

        public ProvisioningConfigurationRepository(
            SCIMDbContext dbContext,
            IMongoClient mongoClient,
            IOptions<MongoDbOptions> options)
        {
            _dbContext = dbContext;
            _mongoClient = mongoClient;
            _options = options.Value;
        }

        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            if (_options.SupportTransaction)
            {
                var session = await _mongoClient.StartSessionAsync(null, token);
                session.StartTransaction();
                return new MongoDbTransaction(session);
            }

            return new MongoDbTransaction();
        }

        public async Task<IEnumerable<ProvisioningConfiguration>> GetAll(CancellationToken cancellationToken)
        {
            var collection = _dbContext.ProvisioningConfigurationLst;
            var result = await collection.AsQueryable().ToMongoListAsync();
            return result;
        }

        public async Task<bool> Update(ProvisioningConfiguration provisioningConfiguration, CancellationToken cancellationToken)
        {
            var update = Builders<ProvisioningConfiguration>.Update.Set(s => s, provisioningConfiguration);
            await _dbContext.ProvisioningConfigurationLst.UpdateOneAsync(a => a.Id == provisioningConfiguration.Id, update);
            return true;
        }
    }
}
