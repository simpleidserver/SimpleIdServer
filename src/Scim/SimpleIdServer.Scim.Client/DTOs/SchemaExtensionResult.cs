// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class SchemaExtensionResult
    {
        [JsonPropertyName("schema")]
        public string Schema { get; set; }
        [JsonPropertyName("required")]
        public bool Required { get; set; }
    }
}
