// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public abstract class BaseProvisioningDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly IIdentityProvisioningStore _identityProvisioningStore;
    private readonly IRealmRepository _realmRepository;

    protected BaseProvisioningDataseeder(IIdentityProvisioningStore identityProvisioningStore, IRealmRepository realmRepository, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _identityProvisioningStore = identityProvisioningStore;
        _realmRepository = realmRepository;
    }

    protected async Task<bool> TryAddProvisioningDef(IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var existingProvisioningDef = await _identityProvisioningStore.GetDefinitionByName(definition.Name, cancellationToken);
        if (existingProvisioningDef == null)
        {
            _identityProvisioningStore.Add(definition);
            return true;
        }

        return false;
    }
}
