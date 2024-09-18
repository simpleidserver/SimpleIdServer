// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.IdentityProvider.Services;
using SimpleIdServer.FastFed.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

public class FastFedDiscoveryController : Controller
{
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IFastFedService _fastFedService;
    private readonly IEnumerable<IIdProviderProvisioningService> _idProviderProvisioningServices;

    public FastFedDiscoveryController(IProviderFederationStore providerFederationStore, IFastFedService fastFedService, IEnumerable<IIdProviderProvisioningService> idProviderProvisioningServices)
    {
        _providerFederationStore = providerFederationStore;
        _fastFedService = fastFedService;
        _idProviderProvisioningServices=  idProviderProvisioningServices;
    }

    [HttpGet]
    public async Task<IActionResult> Confirm(string id, CancellationToken cancellationToken)
    {
        var providerFederation = await _providerFederationStore.Get(HttpUtility.UrlDecode(id), cancellationToken);
        return View(new FastFedDiscoveryConfirmViewModel { IdProviderFederation = providerFederation });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(FastFedDiscoveryConfirmViewModel viewModel, CancellationToken cancellationToken)
    {
        var result = await _fastFedService.StartHandshakeRegistration(viewModel.EntityId, cancellationToken);
        if(result.HasError)
        {
            ModelState.AddModelError(result.ErrorCode, result.ErrorDescriptions.First());
            return View(viewModel);
        }

        var service = _idProviderProvisioningServices.Single(i => i.Name == result.Result.LastCapabilities.ProvisioningProfiles.First());
        return RedirectToAction("Configure", "FastFedAppProvider", new { area = service.Area, entityId = viewModel.EntityId });
    }
}