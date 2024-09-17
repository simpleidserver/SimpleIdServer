// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Stores;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.UI;

[Area(Constants.Areas.Scim)]
public class FastFedAppProviderController : Controller
{
    private readonly IProviderFederationStore _providerFederationStore;

    public FastFedAppProviderController(IProviderFederationStore providerFederationStore)
    {
        _providerFederationStore = providerFederationStore;
    }

    [HttpGet]
    public async Task<IActionResult> Configure(string entityId, CancellationToken cancellationToken)
    {
        var providerFederation = await _providerFederationStore.Get(entityId, cancellationToken);
        var serializedConfiguration = providerFederation.LastCapabilities.Configurations.Single(c => c.ProfileName == Constants.ProvisioningProfileName && c.IsAuthenticationProfile == false).IdProviderSerializedConfiguration;
        var configuration = string.IsNullOrWhiteSpace(serializedConfiguration) ? new ScimProvisioningConfiguration() : JsonSerializer.Deserialize<ScimProvisioningConfiguration>(serializedConfiguration);
        var viewModel = new FastFedAppProviderConfigureViewModel
        {
            EntityId = entityId,
            Configuration = configuration
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Configure(FastFedAppProviderConfigureViewModel viewModel, CancellationToken cancellationToken)
    {
        var providerFederation = await _providerFederationStore.Get(viewModel.EntityId, cancellationToken);
        var lastCapabilities = providerFederation.LastCapabilities;
        var configuration = lastCapabilities.Configurations.Single(c => c.ProfileName == Constants.ProvisioningProfileName);
        configuration.IdProviderSerializedConfiguration = JsonSerializer.Serialize(viewModel.Configuration);
        await _providerFederationStore.SaveChanges(cancellationToken);
        return null;
    }
}