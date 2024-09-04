// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.Apis.ProviderMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Models;
using SimpleIdServer.FastFed.ApplicationProvider.Stores;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.Handshake;

public class HandshakeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IIdentityProviderFederationStore _identityProviderFederationStore;
    private readonly IGetApplicationProviderMetadataQuery _getApplicationProviderMetadataQuery;

    public HandshakeController(
        IHttpClientFactory httpClientFactory,
        IIdentityProviderFederationStore identityProviderFederationStore)
    {
        _httpClientFactory = httpClientFactory;
        _identityProviderFederationStore = identityProviderFederationStore;
    }

    [HttpPost]
    public async Task<IActionResult> Start([FromBody] StartHandshakeRequest request, CancellationToken cancellationToken)
    {
        await Validate(request, cancellationToken);
        IdentityProviderFederation identityProviderFederation = null;
        // 7.2.1.6.Application Provider Whitelists the Identity Provider
        // TODO !!!
        return null;
    }

    private async Task Validate(StartHandshakeRequest request, CancellationToken cancellationToken)
    {
        if (request == null) return; // TODO : ERROR
        if(string.IsNullOrWhiteSpace(request.IdentityProviderUrl)) return; // TODO : ERROR
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            // 7.2.1.2. Application Provider Reads Identity Provider Metadata
            var providerMetadata = await httpClient.GetFromJsonAsync<FastFed.Models.ProviderMetadata>($"{request.IdentityProviderUrl}/provider-metadata", cancellationToken);
            if (providerMetadata == null) return; // TODO : ERROR
            if (providerMetadata.IdentityProvider == null) return; // TODO : ERROR
            var errors = providerMetadata.IdentityProvider.Validate();
            if (errors.Any()) return; // TODO : ERROR
            // Application Provider MUST validate the provider_domain
            if (!IsProviderNameSuffixValid(request, providerMetadata.IdentityProvider)) return; // TODO : ERROR
            // 7.2.1.3. Application Provider Checks For Duplicates
            var identityProviderFederation = await _identityProviderFederationStore.Get(providerMetadata.IdentityProvider.EntityId, cancellationToken);
            if( identityProviderFederation != null && identityProviderFederation.IsActive) return; // TODO : ERROR
            // 7.2.1.4. Application Provider Verifies Compatibility with Identity Provider
            var compatiblityCheckResult = CheckCompatibility(providerMetadata.IdentityProvider);
            if (!compatiblityCheckResult.Any()) return; // TODO : ERROR
            // 7.2.1.5. Application Provider Obtains Confirmation from Administrator
            if (identityProviderFederation == null || !identityProviderFederation.IsConfirmedByAdministrator) return; // TODO : ERROR
        }
    }

    private bool IsProviderNameSuffixValid(StartHandshakeRequest request, FastFed.Models.IdentityProviderMetadata identityProviderMetadata)
    {
        var regex = new Regex($"^https://.*{identityProviderMetadata.ProviderName}.*$");
        return regex.IsMatch(request.IdentityProviderUrl);
    }

    private async Task<bool> IsIdentityProviderFederationExists(FastFed.Models.IdentityProviderMetadata identityProviderMetadata, CancellationToken cancellationToken)
    {
        var identityProviderFederation = await _identityProviderFederationStore.Get(identityProviderMetadata.EntityId, cancellationToken);
        return identityProviderFederation != null;
    }

    private List<string> CheckCompatibility(FastFed.Models.IdentityProviderMetadata identityProviderMetadata)
    {
        var applicationProvider = _getApplicationProviderMetadataQuery.Get();
        return applicationProvider.ApplicationProvider.CheckCompatibility(identityProviderMetadata.Capabilities);
    }
}
