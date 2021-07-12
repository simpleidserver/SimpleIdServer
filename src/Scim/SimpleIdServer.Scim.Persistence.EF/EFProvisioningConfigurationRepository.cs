// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFProvisioningConfigurationRepository : IProvisioningConfigurationRepository
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

        public EFProvisioningConfigurationRepository(SCIMDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(token);
            return new EFTransaction(_dbContext, transaction);
        }

        public async Task<IEnumerable<ProvisioningConfiguration>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<ProvisioningConfiguration> result = await _dbContext.ProvisioningConfigurations.ToListAsync(cancellationToken);
            return result;
        }

        public Task<bool> Update(ProvisioningConfiguration provisioningConfiguration, CancellationToken cancellationToken)
        {
            _dbContext.ProvisioningConfigurations.Update(provisioningConfiguration);
            return Task.FromResult(true);
        }

        public Task<ProvisioningConfiguration> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.ProvisioningConfigurations.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<ProvisioningConfigurationResult> GetQuery(string id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ProvisioningConfigurations.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            return result == null ? null : ProvisioningConfigurationResult.ToDto(result);
        }

        public async Task<SearchResult<ProvisioningConfigurationResult>> SearchConfigurations(SearchProvisioningConfigurationParameter parameter, CancellationToken cancellationToken)
        {
            IQueryable<ProvisioningConfiguration> result = _dbContext.ProvisioningConfigurations;
            if (MAPPING_PROVISIONING_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_PROVISIONING_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = await result.CountAsync(cancellationToken);
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<ProvisioningConfiguration> content = await result.ToListAsync(cancellationToken);
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
            IQueryable<ProvisioningConfigurationHistory> result = _dbContext.ProvisioningConfigurationHistory;
            if (MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = await result.CountAsync(cancellationToken);
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<ProvisioningConfigurationHistory> content = await result.ToListAsync(cancellationToken);
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
