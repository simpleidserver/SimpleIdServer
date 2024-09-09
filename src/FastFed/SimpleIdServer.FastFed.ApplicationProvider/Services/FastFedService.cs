// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.ApplicationProvider.Apis.ProviderMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Models;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;
using SimpleIdServer.FastFed.ApplicationProvider.Resources;
using SimpleIdServer.FastFed.ApplicationProvider.Stores;
using SimpleIdServer.FastFed.Requests;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.Webfinger.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Services;

public interface IFastFedService
{
    Task<ValidationResult<GetWebfingerResult>> ResolveProviders(string email, CancellationToken cancellationToken);
    Task<ValidationResult<(FastFed.Models.ProviderMetadata metadata, IdentityProviderFederation federation)>> ValidateIdentityProviderMetadata(string url, CancellationToken cancellationToken);
    Task<ValidationResult<(StartHandshakeRequest request, string fastFedHandshakeStartUri)>> StartWhitelist(string issuer, string url, CancellationToken cancellationToken);
}

public class FastFedService : IFastFedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IIdentityProviderFederationStore _identityProviderFederationStore;
    private readonly IGetApplicationProviderMetadataQuery _getApplicationProviderMetadataQuery;
    private readonly IWebfingerUrlResolver _webfingerUrlResolver;
    private readonly IWebfingerClientFactory _webfingerClientFactory;
    private readonly FastFedApplicationProviderOptions _options;

    public FastFedService(
        IHttpClientFactory httpClientFactory, 
        IIdentityProviderFederationStore identityProviderFederationStore, 
        IGetApplicationProviderMetadataQuery getApplicationProviderMetadataQuery,
        IWebfingerUrlResolver webfingerUrlResolver,
        IWebfingerClientFactory webfingerClientFactory,
        IOptions<FastFedApplicationProviderOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _identityProviderFederationStore = identityProviderFederationStore;
        _getApplicationProviderMetadataQuery = getApplicationProviderMetadataQuery;
        _webfingerUrlResolver = webfingerUrlResolver;
        _webfingerClientFactory = webfingerClientFactory;
        _options = options.Value;
    }

    public async Task<ValidationResult<GetWebfingerResult>> ResolveProviders(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email)) return ValidationResult<GetWebfingerResult>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.MissingParameter, "email"));
        var attr = new EmailAddressAttribute();
        if(!attr.IsValid(email)) return ValidationResult<GetWebfingerResult>.Fail(ErrorCodes.InvalidRequest, Global.EmailIsInvalid);
        var splittedEmail = email.Split("@");
        var url = _webfingerUrlResolver.Resolve(splittedEmail.Last());
        var client = _webfingerClientFactory.Build();
        var webfingerResult = await client.Get(url, new GetWebfingerRequest
        {
            Rel = new List<string>
            {
                FastFed.Constants.FastFedRel
            },
            Resource = $"acct:{email}"
        }, cancellationToken);
        return ValidationResult<GetWebfingerResult>.Ok(webfingerResult);
    }

    public async Task<ValidationResult<(FastFed.Models.ProviderMetadata metadata, IdentityProviderFederation federation)>> ValidateIdentityProviderMetadata(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url)) return null; // TODO : ERROR
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            // 7.2.1.2. Application Provider Reads Identity Provider Metadata
            var providerMetadata = await httpClient.GetFromJsonAsync<FastFed.Models.ProviderMetadata>($"{url}/provider-metadata", cancellationToken);
            if (providerMetadata == null) return null; // TODO : ERROR
            if (providerMetadata.IdentityProvider == null) return null; // TODO : ERROR
            var errors = providerMetadata.IdentityProvider.Validate();
            if (errors.Any()) return null; // TODO : ERROR
            // Application Provider MUST validate the provider_domain
            if (!IsProviderNameSuffixValid(url, providerMetadata.IdentityProvider)) return null; // TODO : ERROR
            // 7.2.1.3. Application Provider Checks For Duplicates
            var identityProviderFederation = await _identityProviderFederationStore.Get(providerMetadata.IdentityProvider.EntityId, cancellationToken);
            if (identityProviderFederation != null && identityProviderFederation.Status == IdentityProviderStatus.CONFIRMED) return null;
            if (identityProviderFederation != null && identityProviderFederation.IsExpired) return null;
            // 7.2.1.4. Application Provider Verifies Compatibility with Identity Provider
            var compatiblityCheckResult = CheckCompatibility(providerMetadata.IdentityProvider);
            if (!compatiblityCheckResult.Any()) return null; // TODO : ERROR

            /*
            // 7.2.1.5. Application Provider Obtains Confirmation from Administrator
            if (identityProviderFederation == null || !identityProviderFederation.IsConfirmedByAdministrator) return null; // TODO : ERROR
            */

            return ValidationResult<(FastFed.Models.ProviderMetadata, IdentityProviderFederation)>.Ok((providerMetadata, identityProviderFederation));
        }
    }

    public async Task<ValidationResult<(StartHandshakeRequest request, string fastFedHandshakeStartUri)>> StartWhitelist(string issuer, string url, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateIdentityProviderMetadata(url, cancellationToken);
        if(validationResult.HasError)
        {
            return ValidationResult<(StartHandshakeRequest, string)>.Fail(validationResult.ErrorCode, validationResult.ErrorDescription);
        }

        // 7.2.1.6. Application Provider Whitelists the Identity Provider
        await Whitelist(validationResult, cancellationToken);
        // 7.2.1.7. Application Provider Sends Request to the Identity Provider
        var parameter = new StartHandshakeRequest
        {
            AppMetadataUri = $"{issuer}{RouteNames.ProviderMetadata}",
            Expiration = validationResult.Result.federation.ExpirationDateTime
        };
        return ValidationResult<(StartHandshakeRequest, string)>.Ok((parameter, validationResult.Result.metadata.IdentityProvider.FastFedHandshakeStartUri));
    }

    private bool IsProviderNameSuffixValid(string url, FastFed.Models.IdentityProviderMetadata identityProviderMetadata)
    {
        var regex = new Regex($"^https://.*{identityProviderMetadata.ProviderName}.*$");
        return regex.IsMatch(url);
    }

    private List<string> CheckCompatibility(FastFed.Models.IdentityProviderMetadata identityProviderMetadata)
    {
        var applicationProvider = _getApplicationProviderMetadataQuery.Get();
        return applicationProvider.ApplicationProvider.CheckCompatibility(identityProviderMetadata.Capabilities);
    }

    private async Task Whitelist(ValidationResult<(FastFed.Models.ProviderMetadata metadata, IdentityProviderFederation federation)> validationResult, CancellationToken cancellationToken)
    {
        var federation = validationResult.Result.federation;
        var metadata = validationResult.Result.metadata;
        if (federation == null)
        {
            federation = new IdentityProviderFederation
            {
                EntityId = metadata.IdentityProvider.EntityId,
                AuthenticationProfiles = metadata.IdentityProvider.Capabilities.AuthenticationProfiles,
                ProvisioningProfiles = metadata.IdentityProvider.Capabilities.ProvisioningProfiles,
                JwksUri = metadata.IdentityProvider.JwksUri,
                Status = IdentityProviderStatus.WHITELISTED,
                CreateDateTime = DateTime.UtcNow,
                ExpirationDateTime = DateTimeOffset.UtcNow.Add(_options.WhitelistingExpirationTime).ToUnixTimeSeconds()
            };
            _identityProviderFederationStore.Add(federation);
            await _identityProviderFederationStore.SaveChanges(cancellationToken);
        }
    }
}