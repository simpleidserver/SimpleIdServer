// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.FastFed.Apis;
using SimpleIdServer.FastFed.Client;
using SimpleIdServer.FastFed.IdentityProvider.Resources;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Resolvers;
using SimpleIdServer.FastFed.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Services;

public interface IFastFedService
{
    Task<ValidationResult<IdentityProviderFederation>> StartHandshakeRegistration(string entityId, CancellationToken cancellationToken);
}

public class FastFedService : IFastFedService
{
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IIssuerResolver _issuerResolver;
    private readonly IFastFedClientFactory _fastFedClientFactory;
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<IIdProviderProvisioningService> _idProviderProvisioningServices;
    private readonly IEnumerable<IFastFedEnricher> _enrichers;
    private readonly FastFedIdentityProviderOptions _options;
    private readonly ILogger<FastFedService> _logger;

    public FastFedService(
        IProviderFederationStore providerFederationStore, 
        IIssuerResolver issuerResolver, 
        IFastFedClientFactory fastFedClientFactory,
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IEnumerable<IIdProviderProvisioningService> idProviderProvisioningServices,
        IEnumerable<IFastFedEnricher> enrichers,
        IOptions<FastFedIdentityProviderOptions> options,
        ILogger<FastFedService> logger)
    {
        _providerFederationStore = providerFederationStore;
        _issuerResolver = issuerResolver;
        _fastFedClientFactory = fastFedClientFactory;
        _httpClientFactory = httpClientFactory;
        _idProviderProvisioningServices = idProviderProvisioningServices;
        _enrichers = enrichers;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ValidationResult<IdentityProviderFederation>> StartHandshakeRegistration(string entityId, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(entityId)) return ValidationResult<IdentityProviderFederation>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.MissingParameter, nameof(entityId)));
        if (_options.SigningCredentials == null || !_options.SigningCredentials.Any()) return ValidationResult<IdentityProviderFederation>.Fail(ErrorCodes.InvalidRequest, Global.MissingSigningCredential);
        var providerFederation = await _providerFederationStore.Get(entityId, cancellationToken);
        if (providerFederation == null) return ValidationResult<IdentityProviderFederation>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.UnknownProviderFederation, entityId));
        var client = _fastFedClientFactory.Build();
        Domains.ProviderMetadata metadata = null;
        try
        {
            metadata = await client.GetProviderMetadata(providerFederation.ProviderMetadataUrl, true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        if (metadata == null) return ValidationResult<IdentityProviderFederation>.Fail(ErrorCodes.InvalidRequest, Global.ProviderMetadataCannotBeDownloaded);
        if (metadata.ApplicationProvider == null) return ValidationResult<IdentityProviderFederation>.Fail(ErrorCodes.InvalidRequest, Global.ApplicationProviderMetadataCannotBeRetrieved);
        var lastCapabilities = providerFederation.LastCapabilities;
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            Issuer = _issuerResolver.Get(),
            Audience = entityId,
            SigningCredentials = _options.SigningCredentials.First()
        };
        var claims = new Dictionary<string, object>();
        if(lastCapabilities.ProvisioningProfiles != null && lastCapabilities.ProvisioningProfiles.Any())
            claims.Add("provisioning_profiles", lastCapabilities.ProvisioningProfiles);
        if (lastCapabilities.AuthenticationProfiles != null && lastCapabilities.AuthenticationProfiles.Any())
            claims.Add("authentication_profiles", lastCapabilities.AuthenticationProfiles);
        foreach(var enricher in _enrichers)
            enricher.EnrichFastFedRequest(claims);

        securityTokenDescriptor.Claims = claims;
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(securityTokenDescriptor);
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(metadata.ApplicationProvider.FastfedHandshakeRegisterUri),
                Content = new StringContent(token, Encoding.UTF8, "application/jwt")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            if(!httpResult.IsSuccessStatusCode)
            {
                var errorResult = JsonSerializer.Deserialize<ErrorResult>(json);
                return ValidationResult<IdentityProviderFederation>.Fail(errorResult.ErrorCode, errorResult.ErrorDescriptions.First());
            }

            var jsonObj = JsonObject.Parse(json).AsObject();
            foreach (var rec in jsonObj)
            {
                var conf = providerFederation.LastCapabilities.Configurations.SingleOrDefault(c => c.ProfileName == rec.Key);
                if (conf == null) continue;
                conf.AppProviderHandshakeRegisterConfiguration = jsonObj[rec.Key].ToJsonString();
            }

            providerFederation.LastCapabilities.Status = Models.IdentityProviderStatus.CONFIRMED;
            await _providerFederationStore.SaveChanges(cancellationToken);
        }

        return ValidationResult<IdentityProviderFederation>.Ok(providerFederation);
    }
}
