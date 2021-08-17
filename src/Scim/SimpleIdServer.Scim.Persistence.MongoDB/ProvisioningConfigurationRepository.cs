// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class ProvisioningConfigurationRepository : IProvisioningConfigurationRepository
    {
        private static Dictionary<string, string> MAPPING_PROVISIONING_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "updateDateTime", "UpdateDateTime" },
            { "resourceType", "ResourceType" }
        };
        private static Dictionary<string, string> MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "executionDateTime", "ExecutionDateTime" },
            { "representationId", "RepresentationId" },
            { "representationVersion", "RepresentationVersion" }
        };
        private readonly SCIMDbContext _dbContext;
        private readonly IMongoClient _mongoClient;
        private readonly MongoDbOptions _options;
        private IClientSessionHandle _session;

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
                _session = await _mongoClient.StartSessionAsync(null, token);
                _session.StartTransaction();
                return new MongoDbTransaction(_session);
            }

            _session = null;
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
            if (_session != null)
            {
                await _dbContext.ProvisioningConfigurationLst.UpdateOneAsync(_session, a => a.Id == provisioningConfiguration.Id, update);
            }
            else
            {
                await _dbContext.ProvisioningConfigurationLst.UpdateOneAsync(a => a.Id == provisioningConfiguration.Id, update);
            }

            return true;
        }

        public Task<ProvisioningConfiguration> Get(string id, CancellationToken cancellationToken)
        {
            var collection = _dbContext.ProvisioningConfigurationLst;
            return collection.AsQueryable().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<ProvisioningConfigurationResult> GetQuery(string id, CancellationToken cancellationToken)
        {
            var collection = _dbContext.ProvisioningConfigurationLst;
            var record = await collection.AsQueryable().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return record == null ? null : ProvisioningConfigurationResult.ToDto(record);
        }

        public async Task<SearchResult<ProvisioningConfigurationResult>> SearchConfigurations(SearchProvisioningConfigurationParameter parameter, CancellationToken cancellationToken)
        {
            IQueryable<ProvisioningConfiguration> result = _dbContext.ProvisioningConfigurationLst.AsQueryable();
            if (MAPPING_PROVISIONING_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_PROVISIONING_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<ProvisioningConfiguration> content = await result.ToMongoListAsync();
            return new SearchResult<ProvisioningConfigurationResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content.Select(r => ProvisioningConfigurationResult.ToDto(r)).ToList()
            };
        }

        public async Task<SearchResult<ProvisioningConfigurationHistoryResult>> SearchHistory(SearchProvisioningHistoryParameter parameter, CancellationToken cancellationToken)
        {
            var collection = _dbContext.ProvisioningConfigurationLst.AsQueryable();
            var histories = collection.SelectMany(c => c.HistoryLst).AsQueryable();
            if (MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                histories = histories.InvokeOrderBy(MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = histories.Count();
            histories = histories.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<ProvisioningConfigurationHistory> content = await histories.ToMongoListAsync();
            return new SearchResult<ProvisioningConfigurationHistoryResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content.Select(r => ProvisioningConfigurationHistoryResult.ToDto(r)).ToList()
            };
        }
    }
}
