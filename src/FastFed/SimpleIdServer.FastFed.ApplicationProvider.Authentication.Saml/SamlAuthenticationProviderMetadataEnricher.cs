// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public class SamlAuthenticationProviderMetadataEnricher : IProviderMetadataEnricher
{
    private readonly SamlAuthenticationOptions _options;

    public SamlAuthenticationProviderMetadataEnricher(IOptions<SamlAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public void EnrichApplicationProvider(JsonObject otherParameters)
    {
        if (_options.Mappings != null)
            otherParameters.Add(SimpleIdServer.FastFed.Authentication.Saml.Constants.ProvisioningProfileName, JsonObject.Parse(JsonSerializer.Serialize(_options.Mappings)).AsObject());
    }
}
