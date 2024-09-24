// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

[Authorize(DefaultPolicyNames.IsAdminUser)]
public class ApplicationProvidersController : Controller
{
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IProvisioningProfileImportErrorStore _provisioningProfileImportErrorStore;
    private readonly IProvisioningProfileHistoryStore _provisioningProfileHistoryStore;
    private readonly IEnumerable<IIdProviderProvisioningService> _idProviderProvisioningServices;

    public ApplicationProvidersController
        (IProviderFederationStore providerFederationStore,
        IProvisioningProfileImportErrorStore provisioningProfileImportErrorStore,
        IProvisioningProfileHistoryStore provisioningProfileHistoryStore,
        IEnumerable<IIdProviderProvisioningService> idProviderProvisioningServices)
    {
        _providerFederationStore = providerFederationStore;
        _provisioningProfileImportErrorStore = provisioningProfileImportErrorStore;
        _provisioningProfileHistoryStore = provisioningProfileHistoryStore;
        _idProviderProvisioningServices = idProviderProvisioningServices;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var providerFederations = await _providerFederationStore.GetAll(cancellationToken);
        var result = providerFederations.Select(p => new ApplicationProviderViewModel
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
        var result = await _providerFederationStore.Get(entityId, cancellationToken);
        var provisioningProfiles = result.LastCapabilities?.ProvisioningProfiles;
        var top10Errors = await _provisioningProfileImportErrorStore.GetTop(entityId, 10, cancellationToken);
        var histories = await _provisioningProfileHistoryStore.Get(entityId, cancellationToken);
        var viewModel = new ViewApplicationProviderViewModel
        {
            EntityId = entityId,
            Status = result.LastCapabilities.Status,
            ImportErrors = top10Errors.Select(c => new ImportErrorViewModel
            {
                CreateDateTime = c.CreateDateTime,
                ErrorMessage = c.ErrorMessage,
                ExtractedRepresentationId = c.ExtractedRepresentationId
            }).ToList(),
            ProvisioningProfiles = provisioningProfiles?.Select(p => new ProvisioningProfileViewModel
            {
                ProfileName = p,
                NbRecords = histories.SingleOrDefault(h => h.ProfileName == p)?.NbMigratedRecords ?? 0, 
                Area = _idProviderProvisioningServices.Single(s => s.Name == p).Area
            }).ToList() ?? new List<ProvisioningProfileViewModel>()
        };
        return View(viewModel);
    }
}
