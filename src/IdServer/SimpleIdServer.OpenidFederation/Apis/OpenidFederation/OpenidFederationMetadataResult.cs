// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

[JsonConverter(typeof(OpenidFederationMetadataJsonConverter))]
public class OpenidFederationMetadataResult
{
    [JsonPropertyName("federation_entity")]
    public FederationEntityResult FederationEntity { get; set; }
    public Dictionary<string, JsonObject> OtherParameters { get; set; } = new Dictionary<string, JsonObject>();
}