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
    [JsonPropertyName(IdentityProvisioningNames.Processes)]
    public List<IdentityProvisioningProcessResult> Processes { get; set; } = new List<IdentityProvisioningProcessResult>();
    [JsonPropertyName(IdentityProvisioningNames.Values)]
    public Dictionary<string, string> Values { get; set; }
}

public class IdentityProvisioningProcessResult
{
    [JsonPropertyName(IdentityProvisioningNames.Id)]
    public string Id { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.IsExported)]
    public bool IsExported { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.IsImported)]
    public bool IsImported { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.NbExtractedPages)]
    public int NbExtractedPages { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.NbImportedPages)]
    public int NbImportedPages { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.TotalPageToExtract)]
    public int TotalPageToExtract { get; set; }
    [JsonPropertyName(IdentityProvisioningNames.TotalPageToImport)]
    public int TotalPageToImport { get; set; }
}