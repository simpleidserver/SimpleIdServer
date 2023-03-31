// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class RepresentationResult : BaseRepresentationResult
    {
        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }
        [JsonIgnore]
        public JsonObject AdditionalData { get; set; } = new JsonObject();
    }
}
