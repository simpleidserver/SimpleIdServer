// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultProvisioningConfigurationRepository : IProvisioningConfigurationRepository
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
        private readonly ICollection<ProvisioningConfiguration> _configurations;

        public DefaultProvisioningConfigurationRepository(ICollection<ProvisioningConfiguration> configurations)
        {
            _configurations = configurations;
        }

        public Task<ITransaction> StartTransaction(CancellationToken token)
        {
            ITransaction result = new DefaultTransaction();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ProvisioningConfiguration>> GetAll(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<ProvisioningConfiguration>)_configurations);
        }

        public Task<bool> Update(ProvisioningConfiguration provisioningConfiguration, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<ProvisioningConfiguration> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_configurations.FirstOrDefault(p => p.Id == id));
        }

        public Task<ProvisioningConfigurationResult> GetQuery(string id, CancellationToken cancellationToken)
        {
            var result = _configurations.FirstOrDefault(p => p.Id == id);
            if (result == null)
            {
                return Task.FromResult((ProvisioningConfigurationResult)null);
            }

            return Task.FromResult(ProvisioningConfigurationResult.ToDto(result));
        }

        public Task<SearchResult<ProvisioningConfigurationResult>> SearchConfigurations(SearchProvisioningConfigurationParameter parameter, CancellationToken cancellationToken)
        {
            IQueryable<ProvisioningConfiguration> result = _configurations.AsQueryable();
            if (MAPPING_PROVISIONING_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_PROVISIONING_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<ProvisioningConfigurationResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.Select(r => ProvisioningConfigurationResult.ToDto(r)).ToList()
            });
        }

        public Task<SearchResult<ProvisioningConfigurationHistoryResult>> SearchHistory(SearchProvisioningHistoryParameter parameter, CancellationToken cancellationToken)
        {
            IQueryable<ProvisioningConfigurationHistory> result = _configurations.SelectMany(c => c.HistoryLst).AsQueryable();
            if (MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_PROVISIONINGHISTORY_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<ProvisioningConfigurationHistoryResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.Select(r => ProvisioningConfigurationHistoryResult.ToDto(r)).ToList()
            });
        }
    }
}
