// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs;

public class SCIMErrorRepresentation
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("scimType")]
    public string ScimType { get; set; }
    [JsonPropertyName("detail")]
    public string Detail { get; set; }
}
