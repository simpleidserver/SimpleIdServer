// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Provisioning;

public class IdentityProvisioningDefinitionResult
{
    [JsonPropertyName(IdentityProvisioningDefinitionNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(IdentityProvisioningDefinitionNames.Description)]
    public string? Description { get; set; } = null;
    [JsonPropertyName(IdentityProvisioningDefinitionNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningDefinitionNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningDefinitionNames.MappingRules)]
    public ICollection<IdentityProvisioningMappingRuleResult> MappingRules { get; set; } = new List<IdentityProvisioningMappingRuleResult>();
    [JsonPropertyName(IdentityProvisioningDefinitionNames.OptionsName)]
    public string OptionsName { get; set; }
}
