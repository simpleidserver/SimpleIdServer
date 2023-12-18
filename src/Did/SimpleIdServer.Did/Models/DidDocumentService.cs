// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Models
{
    public class DidDocumentService
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
