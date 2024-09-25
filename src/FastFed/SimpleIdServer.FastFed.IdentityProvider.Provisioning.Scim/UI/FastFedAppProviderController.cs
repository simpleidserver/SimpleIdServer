// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Stores;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
        var providerFederation = await _providerFederationStore.Get(HttpUtility.UrlDecode(entityId), cancellationToken);
        var serializedConfiguration = providerFederation.LastCapabilities.Configurations.Single(c => c.ProfileName == FastFed.Provisioning.Scim.Constants.ProvisioningProfileName && c.IsAuthenticationProfile == false).IdProviderConfiguration;
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
        var providerFederation = await _providerFederationStore.Get(HttpUtility.UrlDecode(viewModel.EntityId), cancellationToken);
        var lastCapabilities = providerFederation.LastCapabilities;
        var configuration = lastCapabilities.Configurations.Single(c => c.ProfileName == FastFed.Provisioning.Scim.Constants.ProvisioningProfileName);
        configuration.IdProviderConfiguration = JsonSerializer.Serialize(viewModel.Configuration);
        await _providerFederationStore.SaveChanges(cancellationToken);
        viewModel.IsConfirmed = true;
        return View(viewModel);
    }
}