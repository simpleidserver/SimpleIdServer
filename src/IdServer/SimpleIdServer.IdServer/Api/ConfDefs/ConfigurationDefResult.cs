// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.ConfDefs;

public class ConfigurationDefResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("create_datetime")]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName("update_datetime")]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName("properties")]
    public ICollection<ConfigurationDefRecordResult> Properties { get; set; } = new List<ConfigurationDefRecordResult>();
}