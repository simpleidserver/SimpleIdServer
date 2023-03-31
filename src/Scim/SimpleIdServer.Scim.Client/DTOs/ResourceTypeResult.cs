// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class ResourceTypeResult : BaseRepresentationResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }
        [JsonPropertyName("schema")]
        public string Schema { get; set; }
        [JsonPropertyName("schemaExtensions")]
        public IEnumerable<SchemaExtensionResult> SchemaExtensions { get; set; }
    }
}
