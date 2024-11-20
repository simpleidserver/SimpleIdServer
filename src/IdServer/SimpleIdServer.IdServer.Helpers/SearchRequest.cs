// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Helpers;

public class SearchRequest
{
    [JsonPropertyName("filter")]
    public string Filter { get; set; }
    [JsonPropertyName("orderby")]
    public string OrderBy { get; set; }
    [JsonPropertyName("skip")]
    public int? Skip { get; set; }
    [JsonPropertyName("take")]
    public int? Take { get; set; }
}
