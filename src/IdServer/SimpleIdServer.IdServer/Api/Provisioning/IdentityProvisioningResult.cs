// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Provisioning;

public class IdentityProvisioningResult
{
    [JsonPropertyName(IdentityProvisioningNames.Id)]
    public string Id { get; set; } = null!;
    [JsonPropertyName(IdentityProvisioningNames.Name)]
    public string? Name { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.Description)]
    public string? Description { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.IsEnabled)]
    public bool IsEnabled { get; set; } = true;
    [JsonPropertyName(IdentityProvisioningNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.Definition)]
    public IdentityProvisioningDefinitionResult Definition { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.Histories)]
    public List<IdentityProvisioningHistoryResult> Histories { get; set; } = new List<IdentityProvisioningHistoryResult>();
    [JsonPropertyName(IdentityProvisioningNames.Values)]
    public Dictionary<string, string> Values { get; set; }
}
