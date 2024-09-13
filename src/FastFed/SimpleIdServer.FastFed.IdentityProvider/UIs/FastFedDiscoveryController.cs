// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

public class FastFedDiscoveryController : Controller
{
    private readonly IProviderFederationStore _providerFederationStore;

    public FastFedDiscoveryController(IProviderFederationStore providerFederationStore)
    {
        _providerFederationStore = providerFederationStore;
    }

    [HttpGet]
    public async Task<IActionResult> Confirm(string id, CancellationToken cancellationToken)
    {
        var providerFederation = await _providerFederationStore.Get(id, cancellationToken);
        return View(new FastFedDiscoveryConfirmViewModel { IdProviderFederation = providerFederation });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(FastFedDiscoveryConfirmViewModel viewModel, CancellationToken cancellationToken)
    {

        return null;
    }
}