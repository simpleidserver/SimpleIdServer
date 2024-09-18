// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Stores;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class ScimIdProviderProvisioningService : IIdProviderProvisioningService
{
    private readonly IExtractedRepresentationStore _extractedRepresentationStore;
    private readonly IProviderFederationStore _providerFederationStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public ScimIdProviderProvisioningService(
        IExtractedRepresentationStore extractedRepresentationStore, 
        IProviderFederationStore providerFederationStore,
        IHttpClientFactory httpClientFactory)
    {
        _extractedRepresentationStore = extractedRepresentationStore;
        _providerFederationStore = providerFederationStore;
        _httpClientFactory = httpClientFactory;
    }

    public string Name => Constants.ProvisioningProfileName;
    
    public string Area => Constants.Areas.Scim;
    
    public async Task Migrate(ProvisioningProfileHistory provisioningProfileHistory, CapabilitySettings settings, CancellationToken cancellationToken)
    {
        var mappings = JsonSerializer.Deserialize<AppProviderProvisioningProfileMappingsResult>(settings.AppProviderSerializedConfiguration);
        var configuration = JsonSerializer.Deserialize<AppProviderProvisioningProfileConfigurationResult>(settings.AppProviderSerializedMappings);
        var extractedRepresentations = await _extractedRepresentationStore.GetRepresentations(provisioningProfileHistory.NbMigratedRecords, cancellationToken);
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            using (var scimClient = new SCIMClient(configuration.ScimServiceUri, httpClient))
            {
                var requiredUserAttributes = mappings.DesiredAttributes.DesiredAttributes.RequiredUserAttributes.Select(u => SCIMFilterParser.Parse(u));
                var optionalUserAttributes = mappings.DesiredAttributes.DesiredAttributes.OptionalUserAttributes.Select(u => SCIMFilterParser.Parse(u));

                foreach (var extractedRepresentation in extractedRepresentations)
                {
                    var j = JsonObject.Parse(extractedRepresentation.SerializedRepresentation);

                    // extract into representation.
                    // extract attributes.
                    // when a user is added in the IdServer.
                    // save the representation the entire representation.
                    // each 30 minutes, transform the representation into SCIM (according to the grammar from application provider).
                    // for each configured application provider
                    // // call the provisioning profile service (scim).
                    // support "desired_attributes" in application provider.
                    // providerfederation : TODO
                }
            }
        }
    }
    

    private SCIMSchema ToDomain(SchemaResourceResult schema)
    {
        return new SCIMSchema
        {
            Id = schema.Id,
            Name = schema.Name,
            Attributes = schema.Attributes.Select(a => new SCIMSchemaAttribute(Guid.NewGuid().ToString())
            {
                Name = a.Name           
            }).ToList()
        };
    }
}
