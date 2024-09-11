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

    public async Task<IActionResult> Confirm(string id, CancellationToken cancellationToken)
    {
        await _providerFederationStore.Get(id, cancellationToken);
        return View();
    }
}