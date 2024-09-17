// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Jobs;

public class ProvisionRepresentationsJob
{
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IProvisioningProfileHistoryStore _provisioningProfileHistoryStore;
    private readonly IEnumerable<IIdProviderProvisioningService> _provisioningServices;

    public ProvisionRepresentationsJob(IProviderFederationStore providerFederationStore, IProvisioningProfileHistoryStore provisioningProfileHistoryStore, IEnumerable<IIdProviderProvisioningService> provisioningServices)
    {
        _providerFederationStore = providerFederationStore;    
        _provisioningProfileHistoryStore = provisioningProfileHistoryStore;
        _provisioningServices = provisioningServices;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var providerFederations = await _providerFederationStore.GetAll(cancellationToken);
        foreach (var providerFederation in providerFederations.Where(f => f.LastCapabilities != null && f.LastCapabilities.Status == Models.IdentityProviderStatus.CONFIRMED))
        {
            foreach(var provisioningProfile in providerFederation.LastCapabilities.ProvisioningProfiles)
            {
                var history = await _provisioningProfileHistoryStore.Get(provisioningProfile, cancellationToken);
                var configuration = providerFederation.LastCapabilities.Configurations.Single(c => c.ProfileName == provisioningProfile);
                var service = _provisioningServices.Single(s => s.Name == provisioningProfile);
                await service.Migrate(history, configuration, cancellationToken);
            }
        }
    }
}
