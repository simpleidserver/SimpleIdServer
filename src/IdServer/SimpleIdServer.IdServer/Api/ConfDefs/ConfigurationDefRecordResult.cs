// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.ConfDefs;

public class ConfigurationDefRecordResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("type")]
    public ConfigurationDefinitionRecordTypes Type { get; set; } = ConfigurationDefinitionRecordTypes.INPUT;
    [JsonPropertyName("order")]
    public int Order { get; set; }
    [JsonPropertyName("displayname")]
    public string DisplayName { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("display_condition")]
    public string DisplayCondition { get; set; }
    [JsonPropertyName("is_required")]
    public bool IsRequired { get; set; }
    [JsonPropertyName("possible_values")]
    public List<ConfigurationDefRecordValueResult> PossibleValues { get; set; } = new List<ConfigurationDefRecordValueResult>();
}