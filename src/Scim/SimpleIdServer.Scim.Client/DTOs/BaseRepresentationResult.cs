// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class BaseRepresentationResult
    {
        [JsonPropertyName("schemas")]
        public IEnumerable<string> Schemas { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("meta")]
        public MetadataResult Meta { get; set; }
    }
}
