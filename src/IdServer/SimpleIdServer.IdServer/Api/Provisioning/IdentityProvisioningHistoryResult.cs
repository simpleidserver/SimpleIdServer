// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Provisioning;

public class IdentityProvisioningHistoryResult
{
    [JsonPropertyName(IdentityProvisioningHistoryNames.StartDateTime)]
    public DateTime StartDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningHistoryNames.EndDateTime)]
    public DateTime? EndDateTime { get; set; }
    [JsonPropertyName(IdentityProvisioningHistoryNames.FolderName)]
    public string? FolderName { get; set; } = null;
    [JsonPropertyName(IdentityProvisioningHistoryNames.NbRepresentations)]
    public int NbRepresentations { get; set; }
    [JsonPropertyName(IdentityProvisioningHistoryNames.Status)]
    public IdentityProvisioningHistoryStatus Status { get; set; }
    [JsonPropertyName(IdentityProvisioningHistoryNames.ErrorMessage)]
    public string? ErrorMessage { get; set; } = null;
}
