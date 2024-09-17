// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class AppProviderProvisioningProfileConfigurationResult
{
    [JsonPropertyName("scim_service_uri")]
    public string ScimServiceUri { get; set; }
}
