// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Provisioning;

public class IdentityProvisioningLaunchedResult
{
    [JsonPropertyName(IdentityProvisioningNames.Id)]
    public string Id { get; set; } = null!;
}
