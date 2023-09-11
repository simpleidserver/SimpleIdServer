// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.DTOs;

public class SearchResult<TContent>
{
    [JsonPropertyName("content")]
    public ICollection<TContent> Content { get; set; }
    [JsonPropertyName("count")]
    public int Count { get; set; }
}
