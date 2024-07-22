// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Domains;

public class FederationEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = null!;
    [JsonPropertyName("realm")]
    public string? Realm { get; set; } = null;
    [JsonPropertyName("is_subordinate")]
    public bool IsSubordinate { get; set; } = false;
    [JsonPropertyName("create_datetime")]
    public DateTime CreateDateTime { get; set; }
}