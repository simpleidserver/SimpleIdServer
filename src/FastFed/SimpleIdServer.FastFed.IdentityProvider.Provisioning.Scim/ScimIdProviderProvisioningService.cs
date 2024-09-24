// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Resources;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Provisioning.Scim;
using SimpleIdServer.FastFed.Stores;
using SimpleIdServer.Scim.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class ScimIdProviderProvisioningService : IIdProviderProvisioningService
{
    private readonly IExtractedRepresentationStore _extractedRepresentationStore;
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;

    public ScimIdProviderProvisioningService(
        IExtractedRepresentationStore extractedRepresentationStore, 
        IProviderFederationStore providerFederationStore,
        IdServer.Helpers.IHttpClientFactory httpClientFactory)
    {
        _extractedRepresentationStore = extractedRepresentationStore;
        _providerFederationStore = providerFederationStore;
        _httpClientFactory = httpClientFactory;
    }

    public string Name => SimpleIdServer.FastFed.Provisioning.Scim.Constants.ProvisioningProfileName;
    
    public string Area => Constants.Areas.Scim;
    
    public async Task<MigrationResult> Migrate(ProvisioningProfileHistory provisioningProfileHistory, CapabilitySettings settings, CancellationToken cancellationToken)
    {
        var result = new MigrationResult(); ;
        if (string.IsNullOrWhiteSpace(settings.IdProviderConfiguration) || string.IsNullOrWhiteSpace(settings.AppProviderConfiguration)) return result;
        var mappings = JsonSerializer.Deserialize<ScimEntrepriseMappingsResult>(settings.AppProviderConfiguration);
        var configuration = JsonSerializer.Deserialize<ScimEntrepriseRegistrationResult>(settings.AppProviderHandshakeRegisterConfiguration);
        var scimProvisioningConfiguration = JsonSerializer.Deserialize<ScimProvisioningConfiguration>(settings.IdProviderConfiguration);
        var extractedRepresentations = await _extractedRepresentationStore.GetRepresentations(provisioningProfileHistory.NbMigratedRecords, cancellationToken);
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var accessToken = await GetScimAccessToken(httpClient, configuration, scimProvisioningConfiguration, cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken)) return result;
            using (var scimClient = new SCIMClient(configuration.ScimServiceUri, httpClient))
            {
                foreach (var extractedRepresentation in extractedRepresentations)
                {
                    switch (extractedRepresentation.Operation)
                    {
                        case ExtractedRepresentationOperations.NEW:
                            var jsonObject = JsonObject.Parse(extractedRepresentation.SerializedRepresentation);
                            if(extractedRepresentation.Operation != ExtractedRepresentationOperations.NEW)
                            {
                                result.Errors.Add(new ProvisioningProfileImportError
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    CreateDateTime = DateTime.UtcNow,
                                    ErrorMessage = Global.OnlyNewOperationIsSupported,
                                    ExtractedRepresentationId = extractedRepresentation.Id,
                                    EntityId = provisioningProfileHistory.EntityId,
                                    ProfileName = Name
                                });
                                return result;
                            }

                            var extractionResult = SCIMRequestExtractor.ExtractAdd(jsonObject.AsObject(), mappings.DesiredAttributes.Attrs);
                            if(extractionResult.HasError)
                            {
                                result.Errors.AddRange(extractionResult.ErrorMessages.Select(e => new ProvisioningProfileImportError
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    CreateDateTime = DateTime.UtcNow,
                                    ErrorMessage = e,
                                    ExtractedRepresentationId = extractedRepresentation.Id,
                                    EntityId = provisioningProfileHistory.EntityId,
                                    ProfileName = Name
                                }));
                                return result;
                            }

                            await scimClient.AddUser(extractionResult.Result, accessToken, cancellationToken);
                            result.NbMigratedRepresentation++;
                            break;
                    }
                }
            }
        }

        return result;
    }

    private async Task<string> GetScimAccessToken(HttpClient httpClient, ScimEntrepriseRegistrationResult configuration, ScimProvisioningConfiguration scimProvisioningConfiguration, CancellationToken cancellationToken)
    {
        if (scimProvisioningConfiguration.AuthenticationType == AuthenticationTypes.APIKEY) return scimProvisioningConfiguration.ApiSecret;
        var dic = new Dictionary<string, string>
        {
            { "client_id", scimProvisioningConfiguration.ClientId },
            { "client_secret", scimProvisioningConfiguration.ClientSecret },
            { "scope", configuration.JwtProfile.Scope },
            { "grant_type", "client_credentials" }
        };
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(dic),
            RequestUri = new Uri(configuration.JwtProfile.TokenEndpoint)
        };
        var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
        if (!httpResult.IsSuccessStatusCode) return null;
        var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
        var jsonObject = JsonObject.Parse(json);
        return jsonObject["access_token"].ToString();
    }
}
