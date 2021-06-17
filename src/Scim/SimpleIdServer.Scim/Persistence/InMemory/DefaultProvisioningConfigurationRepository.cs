// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultProvisioningConfigurationRepository : IProvisioningConfigurationRepository
    {
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
    }
}
