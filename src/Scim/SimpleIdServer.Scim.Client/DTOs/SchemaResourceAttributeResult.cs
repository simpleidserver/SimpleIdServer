// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs;

public class SchemaResourceAttributeResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("multiValued")]
    public bool MultiValued {  get; set; }
    [JsonPropertyName("subAttributes")]
    public ICollection<SchemaResourceAttributeResult> SubAttributes { get; set; }
}
