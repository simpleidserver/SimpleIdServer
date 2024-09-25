// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.Domains;
using SimpleIdServer.FastFed.Resolvers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class ScimGetProviderMetadataQuery : GetProviderMetadataQuery
{
    private readonly ScimProvisioningOptions _scimProvisioningOptions;

    public ScimGetProviderMetadataQuery(
        IIssuerResolver issuerResolver, 
        IOptions<FastFedOptions> options,
        IOptions<ScimProvisioningOptions> scimProvisioningOptions) : base(issuerResolver, options)
    {
        _scimProvisioningOptions = scimProvisioningOptions.Value;
    }

    public override ProviderMetadata Get()
    {
        var result = base.Get();
        if (_scimProvisioningOptions.Mappings != null)
            result.ApplicationProvider.OtherParameters.Add(SimpleIdServer.FastFed.Provisioning.Scim.Constants.ProvisioningProfileName, JsonObject.Parse(JsonSerializer.Serialize(_scimProvisioningOptions.Mappings)).AsObject());

        return result;
    }
}
