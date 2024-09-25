// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Hangfire;
using SimpleIdServer.FastFed.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.FastFed.IdentityProvider.Jobs;

public class ProvisionRepresentationsJob
{
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IProvisioningProfileHistoryStore _provisioningProfileHistoryStore;
    private readonly IEnumerable<IIdProviderProvisioningService> _provisioningServices;
    private readonly IProvisioningProfileImportErrorStore _provisioningProfileImportErrorStore;

    public ProvisionRepresentationsJob(
        IProviderFederationStore providerFederationStore, 
        IProvisioningProfileHistoryStore provisioningProfileHistoryStore,
        IProvisioningProfileImportErrorStore provisioningProfileImportErrorStore,
        IEnumerable<IIdProviderProvisioningService> provisioningServices)
    {
        _providerFederationStore = providerFederationStore;    
        _provisioningProfileHistoryStore = provisioningProfileHistoryStore;
        _provisioningProfileImportErrorStore = provisioningProfileImportErrorStore;
        _provisioningServices = provisioningServices;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    public async Task Execute(CancellationToken cancellationToken)
    {
        var providerFederations = await _providerFederationStore.GetAll(cancellationToken);
        foreach (var providerFederation in providerFederations.Where(f => f.LastCapabilities != null && f.LastCapabilities.Status == Models.IdentityProviderStatus.CONFIRMED))
            {
                foreach (var provisioningProfile in providerFederation.LastCapabilities.ProvisioningProfiles)
                {
                    var history = await _provisioningProfileHistoryStore.Get(providerFederation.EntityId, provisioningProfile, cancellationToken);
                    if(history == null)
                    {
                        history = new Models.ProvisioningProfileHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            NbMigratedRecords = 0,
                            EntityId = providerFederation.EntityId,
                            ProfileName = provisioningProfile
                        };
                        _provisioningProfileHistoryStore.Add(history);
                    }
                    var configuration = providerFederation.LastCapabilities.Configurations.Single(c => c.ProfileName == provisioningProfile);
                    var service = _provisioningServices.Single(s => s.Name == provisioningProfile);
                    var migrationResult = await service.Migrate(history, configuration, cancellationToken);
                    history.NbMigratedRecords += migrationResult.NbMigratedRepresentation;
                    _provisioningProfileImportErrorStore.Add(migrationResult.Errors);
                }
            }

        await _provisioningProfileImportErrorStore.SaveChanges(cancellationToken);
        await _providerFederationStore.SaveChanges(cancellationToken);
    }
}
