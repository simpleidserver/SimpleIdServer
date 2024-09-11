// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.FastFed.Apis;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.Client;
using SimpleIdServer.FastFed.Requests;
using SimpleIdServer.FastFed.IdentityProvider.Resources;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.FastFed;

// [Authorize("Authenticated")]
public class FastFedController : BaseController
{
    private readonly IFastFedClientFactory _fastFedClientFactory;
    private readonly IGetProviderMetadataQuery _getProviderMetadataQuery;
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly ILogger<FastFedController> _logger;

    public FastFedController(IFastFedClientFactory fastFedClientFactory, IGetProviderMetadataQuery getProviderMetadataQuery, IProviderFederationStore providerFederationStore, ILogger<FastFedController> logger)
    {
        _fastFedClientFactory = fastFedClientFactory;
        _getProviderMetadataQuery = getProviderMetadataQuery;
        _providerFederationStore = providerFederationStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Start([FromQuery] StartHandshakeRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(request, cancellationToken);
        if(validationResult.HasError)
        {
            return BuildResult(validationResult);
        }

        // 7.2.2.4. Identity Provider Checks For Duplicates and Updates
        var providerMetadata = validationResult.Result;
        var providerFederation = await _providerFederationStore.Get(providerMetadata.ApplicationProvider.EntityId, cancellationToken);
        if (providerFederation == null)
        {
            providerFederation = new Models.IdentityProviderFederation
            {
                EntityId = providerMetadata.ApplicationProvider.EntityId,
                CreateDateTime = DateTime.UtcNow,
                Capabilities = new System.Collections.Generic.List<IdentityProviderFederationCapabilities>()
            };
            _providerFederationStore.Add(providerFederation);
        }

        var nextAuthenticationProfiles = providerMetadata.ApplicationProvider.Capabilities.AuthenticationProfiles;
        var nextProvisioningProfiles = providerMetadata.ApplicationProvider.Capabilities.ProvisioningProfiles;
        if(providerFederation.LastCapabilities == null || (providerFederation.LastCapabilities.Status == IdentityProviderStatus.CREATE))
        {
            if(providerFederation.LastCapabilities == null)
            {
                providerFederation.Capabilities.Add(new IdentityProviderFederationCapabilities
                {
                    Id = Guid.NewGuid().ToString(),
                    AuthenticationProfiles = nextAuthenticationProfiles,
                    ProvisioningProfiles = nextProvisioningProfiles,
                    ExpirationDateTime = request.Expiration.Value,
                    Status = IdentityProviderStatus.CREATE,
                    CreateDateTime = DateTime.UtcNow
                });
            }
            else
            {
                providerFederation.LastCapabilities.AuthenticationProfiles = nextAuthenticationProfiles;
                providerFederation.LastCapabilities.ProvisioningProfiles = nextProvisioningProfiles;
                providerFederation.LastCapabilities.ExpirationDateTime = request.Expiration.Value;
            }

            await _providerFederationStore.SaveChanges(cancellationToken);
            return RedirectToAction("Confirm", "FastFedDiscovery", new { id = providerMetadata.ApplicationProvider.EntityId });
        }

        // If new values exist in the capabilities that don't exist in the current configuration, the Administrator SHOULD be given the choice to add or update to the new value. 
        var previousAuthenticationProfiles = providerFederation.LastCapabilities.AuthenticationProfiles;
        var previousProvisioningProfiles = providerFederation.LastCapabilities.ProvisioningProfiles;
        var newAuthenticationProfiles = nextAuthenticationProfiles.Where(p => !previousAuthenticationProfiles.Contains(p));
        var newProvisioningProfiles = nextProvisioningProfiles.Where(p => !previousProvisioningProfiles.Contains(p));
        if(newAuthenticationProfiles.Any() || newProvisioningProfiles.Any())
        {
            providerFederation.Capabilities.Add(new IdentityProviderFederationCapabilities
            {
                Id = Guid.NewGuid().ToString(),
                AuthenticationProfiles = nextAuthenticationProfiles,
                ProvisioningProfiles = nextProvisioningProfiles,
                ExpirationDateTime = request.Expiration.Value,
                Status = IdentityProviderStatus.CREATE
            });
            await _providerFederationStore.SaveChanges(cancellationToken);
            return RedirectToAction("Confirm", "FastFedDiscovery", new { id = providerMetadata.ApplicationProvider.EntityId });
        }

        providerFederation.LastCapabilities.AuthenticationProfiles = nextAuthenticationProfiles;
        providerFederation.LastCapabilities.ProvisioningProfiles = nextProvisioningProfiles;
        providerFederation.LastCapabilities.ExpirationDateTime = request.Expiration.Value;
        await _providerFederationStore.SaveChanges(cancellationToken);
        return null;
    }

    private async Task<ValidationResult<Domains.ProviderMetadata>> Validate(StartHandshakeRequest request, CancellationToken cancellationToken)
    {
        // 7.2.2.1.Identity Provider Authenticates Administrator
        // if (User.IsInRole("administrator")) return null; // NOT AUTHORIZED....
        if (request == null) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, Global.BadIncomingRequest);
        if (string.IsNullOrWhiteSpace(request.AppMetadataUri)) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.MissingParameter, "app_metadata_uri"));
        var client = _fastFedClientFactory.Build();
        Domains.ProviderMetadata metadata = null;
        try
        {
            metadata = await client.GetProviderMetadata(request.AppMetadataUri, true, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        // 7.2.2.2. Identity Provider Reads Application Provider Metadata
        if (metadata == null) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, Global.ProviderMetadataCannotBeDownloaded);
        if (metadata.ApplicationProvider == null) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, Global.ApplicationProviderMetadataCannotBeRetrieved);
        var errors = metadata.ApplicationProvider.Validate();
        if (errors.Any()) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, errors);

        // 7.2.2.3. Identity Provider Verifies Compatibility
        var providerMetadata = _getProviderMetadataQuery.Get();
        var compatiblityCheckResult = providerMetadata.IdentityProvider.CheckCompatibility(metadata.ApplicationProvider.Capabilities);
        if (compatiblityCheckResult.Any()) return ValidationResult<Domains.ProviderMetadata>.Fail(ErrorCodes.InvalidRequest, compatiblityCheckResult);

        return ValidationResult<Domains.ProviderMetadata>.Ok(metadata);
    }
}