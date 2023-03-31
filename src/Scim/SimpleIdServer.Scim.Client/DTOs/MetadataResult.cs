// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class MetadataResult
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }
        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}
