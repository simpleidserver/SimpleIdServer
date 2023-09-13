// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs;

public class SchemaResourceResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("attributes")]
    public ICollection<SchemaResourceAttributeResult> Attributes { get; set; } = new List<SchemaResourceAttributeResult>();
}