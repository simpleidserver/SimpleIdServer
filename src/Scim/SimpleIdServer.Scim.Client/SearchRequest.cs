// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client
{
    public class SearchRequest
    {
        [JsonPropertyName("count")]
        public int Count { get; set; } = 100;
        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }
    }
}
