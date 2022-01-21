// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface IProvisioningConfigurationRepository
    {
        Task<ITransaction> StartTransaction(CancellationToken token);
        Task<IEnumerable<ProvisioningConfiguration>> GetAll(CancellationToken cancellationToken);
        Task<ProvisioningConfiguration> Get(string id, CancellationToken cancellationToken);
        Task<ProvisioningConfigurationResult> GetQuery(string id, CancellationToken cancellationToken);
        Task<SearchResult<ProvisioningConfigurationResult>> SearchConfigurations(SearchProvisioningConfigurationParameter parameter, CancellationToken cancellationToken);
        Task<SearchResult<ProvisioningConfigurationHistoryResult>> SearchHistory(SearchProvisioningHistoryParameter parameter, CancellationToken cancellationToken);
        Task<bool> Update(ProvisioningConfiguration provisioningConfiguration, CancellationToken cancellationToken);
    }
}
