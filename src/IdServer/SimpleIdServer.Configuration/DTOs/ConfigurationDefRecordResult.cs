// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Configuration.DTOs;

public class ConfigurationDefRecordResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("type")]
    public ConfigurationDefinitionRecordTypes Type { get; set; } = ConfigurationDefinitionRecordTypes.INPUT;
    [JsonPropertyName("displayname")]
    public string DisplayName { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("possible_values")]
    public ICollection<ConfigurationDefRecordValueResult> PossibleValues { get; set; } = new List<ConfigurationDefRecordValueResult>();
}