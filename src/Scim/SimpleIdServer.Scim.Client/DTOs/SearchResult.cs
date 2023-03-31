// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.DTOs
{
    public class SearchResult<T> where T : class
    {
        [JsonPropertyName("schemas")]
        public IEnumerable<string> Schemas { get; set; }
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }
        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }
        [JsonPropertyName("Resources")]
        public IEnumerable<T> Resources { get; set; }
    }
}
