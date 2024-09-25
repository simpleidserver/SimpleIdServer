// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;
using SimpleIdServer.FastFed.ApplicationProvider.Resources;
using SimpleIdServer.FastFed.Client;
using SimpleIdServer.FastFed.Domains;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Requests;
using SimpleIdServer.FastFed.Stores;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.Webfinger.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Services;

public interface IFastFedService
{
    Task<ValidationResult<GetWebfingerResult>> ResolveProviders(string email, CancellationToken cancellationToken);
    Task<ValidationResult<(StartHandshakeRequest request, string fastFedHandshakeStartUri)>> StartWhitelist(string issuer, string url, CancellationToken cancellationToken);
    Task<ValidationResult<Dictionary<string, JsonObject>>> Register(string content, CancellationToken cancellationToken);
}

public class FastFedService : IFastFedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProviderFederationStore _identityProviderFederationStore;
    private readonly IGetProviderMetadataQuery _getProviderMetadataQuery;
    private readonly IWebfingerUrlResolver _webfingerUrlResolver;
    private readonly IWebfingerClientFactory _webfingerClientFactory;
    private readonly IFastFedClientFactory _fastFedClientFactory;
    private readonly IEnumerable<IAppProviderProvisioningService> _provisioningServices;
    private readonly ILogger<FastFedService> _logger;
    private readonly FastFedApplicationProviderOptions _options;

    public FastFedService(
        IHttpClientFactory httpClientFactory, 
        IProviderFederationStore identityProviderFederationStore,
        IGetProviderMetadataQuery getProviderMetadataQuery,
        IWebfingerUrlResolver webfingerUrlResolver,
        IWebfingerClientFactory webfingerClientFactory,
        IFastFedClientFactory fastFedClientFactory,
        IEnumerable<IAppProviderProvisioningService> provisioningServices,
        ILogger<FastFedService> logger,
        IOptions<FastFedApplicationProviderOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _identityProviderFederationStore = identityProviderFederationStore;
        _getProviderMetadataQuery = getProviderMetadataQuery;
        _webfingerUrlResolver = webfingerUrlResolver;
        _webfingerClientFactory = webfingerClientFactory;
        _fastFedClientFactory = fastFedClientFactory;
        _provisioningServices = provisioningServices;
        _logger = logger;
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

    public async Task<ValidationResult<(StartHandshakeRequest request, string fastFedHandshakeStartUri)>> StartWhitelist(string issuer, string url, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateIdentityProviderMetadata(url, cancellationToken);
        if(validationResult.HasError)
        {
            return ValidationResult<(StartHandshakeRequest, string)>.Fail(validationResult.ErrorCode, validationResult.ErrorDescriptions);
        }

        // 7.2.1.6. Application Provider Whitelists the Identity Provider
        var federation = await Whitelist(validationResult, cancellationToken);
        // 7.2.1.7. Application Provider Sends Request to the Identity Provider
        var parameter = new StartHandshakeRequest
        {
            AppMetadataUri = $"{issuer}{RouteNames.ProviderMetadata}",
            Expiration = federation.LastCapabilities.ExpirationDateTime
        };
        return ValidationResult<(StartHandshakeRequest, string)>.Ok((parameter, validationResult.Result.metadata.IdentityProvider.FastFedHandshakeStartUri));
    }

    public async Task<ValidationResult<Dictionary<string, JsonObject>>> Register(string content, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content)) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, Global.RegisterRequestCannotBeEmpty);
        var appProviderMetadata = _getProviderMetadataQuery.Get();
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(content)) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, Global.RegisterRequestMustBeJwt);
        var jwt = handler.ReadJsonWebToken(content);
        // 2. Verify the iss attribute matches a whitelisted entity_id for the tenant of the Application Provider, as captured in Section 7.2.1.6.
        var idProviderFederation = await _identityProviderFederationStore.Get(jwt.Issuer, cancellationToken);
        if (idProviderFederation == null) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.CannotCompleteRegistrationForUnknownProvider, jwt.Issuer));
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var jwks = await httpClient.GetFromJsonAsync<JwksResult>(idProviderFederation.JwksUri, cancellationToken);
            // 4. Verify the kid matches an entry in the key set hosted at the whitelisted jwks_uri captured in Section.
            var jwk = jwks.Keys.SingleOrDefault(k => k.Kid == jwt.Kid);
            if (jwk == null) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.JwkKidIsNotFound, jwt.Kid));
            var parameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                IssuerSigningKey = jwk,
                ValidateAudience = false,
                ValidateIssuer = false
            };
            var validationResult = await handler.ValidateTokenAsync(content, parameters);
            // 5. Verify the JWT signature using the key from the key set. The signing algorithm for the kid in the key set MUST match the signing algorithm in the JWT.
            if (!validationResult.IsValid)
            {
                _logger.LogError(validationResult.Exception.ToString());
                return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, validationResult.Exception.ToString());
            }

            // 1. Verify the aud attribute matches the entity_id of a tenant in the Application Provider.
            if (!jwt.Audiences.Contains(appProviderMetadata.ApplicationProvider.EntityId)) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, Global.AudienceDoesntMatchEntityIdOfApplicationProvider);
        }

        // 6. If an expiration date exists on the whitelist (Section 7.2.1.6), verify the expiration date has not been exceeded.
        if (idProviderFederation.LastCapabilities.IsExpired) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, Global.WhitelistingIsExpired);
        // 7. Verify the values of authentication_profiles and provisioning_profiles against the whitelisted capabilities captured in Section 7.2.1.6.
        var authenticationProfiles = jwt.Claims.Where(c => c.Type == "authentication_profiles").Select(c => c.Value).ToList();
        var provisioningProfiles = jwt.Claims.Where(c => c.Type == "provisioning_profiles").Select(c => c.Value).ToList();
        var isCapabilitiesDifferent = authenticationProfiles.Count() != (idProviderFederation.LastCapabilities.AuthenticationProfiles?.Count() ?? 0) ||
            authenticationProfiles.Any(a => !(idProviderFederation.LastCapabilities.AuthenticationProfiles?.Contains(a) ?? false)) ||
            provisioningProfiles.Count() != (idProviderFederation.LastCapabilities.ProvisioningProfiles?.Count() ?? 0) ||
            provisioningProfiles.Any(p => !(idProviderFederation.LastCapabilities.ProvisioningProfiles?.Contains(p) ?? false));
        if (isCapabilitiesDifferent) return ValidationResult<Dictionary<string, JsonObject>>.Fail(ErrorCodes.InvalidRequest, Global.CapabilitiesCannotBeDifferent);
        // Activate the capabilities - provisioning_profile and authentication profile.
        var dic = new Dictionary<string, JsonObject>();
        foreach(var provisioningProfile in provisioningProfiles)
        {
            var service = _provisioningServices.Single(s => s.Name == provisioningProfile);
            var enableResult = await service.EnableCapability(idProviderFederation.EntityId, jwt, cancellationToken);
            dic.Add(service.Name, enableResult);
        }

        idProviderFederation.LastCapabilities.Status = IdentityProviderStatus.CONFIRMED;
        await _identityProviderFederationStore.SaveChanges(cancellationToken);
        return ValidationResult<Dictionary<string, JsonObject>>.Ok(dic);
    }

    private async Task<ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>> ValidateIdentityProviderMetadata(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url)) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.MissingParameter, nameof(url)));
        // 7.2.1.2. Application Provider Reads Identity Provider Metadata
        var client = _fastFedClientFactory.Build();
        ProviderMetadata providerMetadata = null;
        try
        {
            providerMetadata = await client.GetProviderMetadata(url, false, cancellationToken);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.ToString());
        }

        if (providerMetadata == null) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, Global.ProviderMetadataCannotBeRetrieved);
        if (providerMetadata.IdentityProvider == null) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, Global.IdProviderMetadataCannotBeRetrieved);
        var errors = providerMetadata.IdentityProvider.Validate();
        if (errors.Any()) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, errors);
        // Application Provider MUST validate the provider_domain
        if (!IsProviderNameSuffixValid(url, providerMetadata.IdentityProvider)) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.ProviderNameSuffixNotSatisfied, providerMetadata.IdentityProvider.ProviderDomain));
        // 7.2.1.3. Application Provider Checks For Duplicates
        var identityProviderFederation = await _identityProviderFederationStore.Get(providerMetadata.IdentityProvider.EntityId, cancellationToken);
        if (identityProviderFederation != null && identityProviderFederation.LastCapabilities.Status == IdentityProviderStatus.CONFIRMED) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, Global.IdentityProviderFederationExists);
        // 7.2.1.4. Application Provider Verifies Compatibility with Identity Provider
        var compatiblityCheckResult = CheckCompatibility(providerMetadata.IdentityProvider);
        if (compatiblityCheckResult.Any()) return ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)>.Fail(ErrorCodes.InvalidRequest, compatiblityCheckResult);
        return ValidationResult<(FastFed.Domains.ProviderMetadata, IdentityProviderFederation)>.Ok((providerMetadata, identityProviderFederation));
    }

    private bool IsProviderNameSuffixValid(string url, FastFed.Domains.IdentityProviderMetadata identityProviderMetadata)
    {
        var regex = new Regex($"^(https|http)://.*{identityProviderMetadata.ProviderDomain}.*$");
        return regex.IsMatch(url);
    }

    private List<string> CheckCompatibility(FastFed.Domains.IdentityProviderMetadata identityProviderMetadata)
    {
        var applicationProvider = _getProviderMetadataQuery.Get();
        return applicationProvider.ApplicationProvider.CheckCompatibility(identityProviderMetadata.Capabilities);
    }

    private async Task<IdentityProviderFederation> Whitelist(ValidationResult<(FastFed.Domains.ProviderMetadata metadata, IdentityProviderFederation federation)> validationResult, CancellationToken cancellationToken)
    {
        var federation = validationResult.Result.federation;
        var metadata = validationResult.Result.metadata;
        if (federation == null)
        {
            federation = new IdentityProviderFederation
            {
                EntityId = metadata.IdentityProvider.EntityId,
                Capabilities = new List<IdentityProviderFederationCapabilities>
                {
                    new IdentityProviderFederationCapabilities
                    {
                        Id = Guid.NewGuid().ToString(),
                        AuthenticationProfiles = metadata.IdentityProvider.Capabilities.AuthenticationProfiles,
                        ProvisioningProfiles = metadata.IdentityProvider.Capabilities.ProvisioningProfiles,
                        Status = IdentityProviderStatus.WHITELISTED,
                        ExpirationDateTime = DateTimeOffset.UtcNow.Add(_options.WhitelistingExpirationTime).ToUnixTimeSeconds()
                    }
                },
                JwksUri = metadata.IdentityProvider.JwksUri,
                CreateDateTime = DateTime.UtcNow,
            };
            _identityProviderFederationStore.Add(federation);
            await _identityProviderFederationStore.SaveChanges(cancellationToken);
        }

        return federation;
    }
}