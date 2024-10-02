// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class ScimProviderMetadataEnricher : IProviderMetadataEnricher
{
    private readonly ScimProvisioningOptions _scimProvisioningOptions;

    public ScimProviderMetadataEnricher(IOptions<ScimProvisioningOptions> scimProvisioningOptions)
    {
        _scimProvisioningOptions = scimProvisioningOptions.Value;
    }

    public void EnrichApplicationProvider(JsonObject otherParameters)
    {
        if (_scimProvisioningOptions.Mappings != null)
            otherParameters.Add(SimpleIdServer.FastFed.Provisioning.Scim.Constants.ProvisioningProfileName, JsonObject.Parse(JsonSerializer.Serialize(_scimProvisioningOptions.Mappings)).AsObject());
    }
}
