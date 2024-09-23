// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;
using SimpleIdServer.FastFed.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs;

[Authorize(DefaultPolicyNames.IsAdminUser)]
public class IdentityProvidersController : Controller
{
    private readonly IProviderFederationStore _providerFederationStore;

    public IdentityProvidersController(IProviderFederationStore providerFederationStore)
    {
        _providerFederationStore = providerFederationStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var providerFederations = await _providerFederationStore.GetAll(cancellationToken);
        var result = providerFederations.Select(p => new IdentityProviderViewModel
        {
            EntityId = p.EntityId,
            CreateDateTime = p.CreateDateTime,
            Status = p.LastCapabilities?.Status,
            ExpirationTime = p.LastCapabilities?.ExpirationDateTime,
            UpdateDateTime = p.LastCapabilities?.CreateDateTime
        });
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> View(string entityId, CancellationToken cancellationToken)
    {
        var providerFederation = await _providerFederationStore.Get(entityId, cancellationToken);
        var viewModel = new ViewIdentityProviderViewModel
        {
            EntityId = entityId,
            Status = providerFederation.LastCapabilities.Status,
            ProvisioningProfiles = providerFederation.LastCapabilities.ProvisioningProfiles
        };
        return View();
    }
}
