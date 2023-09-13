// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Provisioning;

public class IdentityProvisioningExtractionResult
{
    [JsonPropertyName(IdentityProvisioningExtractionNames.Id)]
    public string Id { get; set; }
    [JsonPropertyName(IdentityProvisioningExtractionNames.Version)]
    public string Version { get; set; }
    [JsonPropertyName(IdentityProvisioningExtractionNames.Values)]
    public List<string> Values { get; set; }
}
