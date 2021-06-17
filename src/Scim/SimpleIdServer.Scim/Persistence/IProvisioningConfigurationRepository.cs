// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface IProvisioningConfigurationRepository
    {
        Task<ITransaction> StartTransaction(CancellationToken token);
        Task<IEnumerable<ProvisioningConfiguration>> GetAll(CancellationToken cancellationToken);
        Task<bool> Update(ProvisioningConfiguration provisioningConfiguration, CancellationToken cancellationToken);
    }
}
