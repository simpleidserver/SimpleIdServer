// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Provisioning;

public class TestConnectionResult
{
    [JsonPropertyName(TestConnectionNames.Columns)]
    public List<string> Columns { get; set; }
    [JsonPropertyName(TestConnectionNames.Values)]
    public ICollection<IdentityProvisioningExtractionResult> Values { get; set; }
}
