// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFProvisioningConfigurationRepository : IProvisioningConfigurationRepository
    {
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
    }
}
