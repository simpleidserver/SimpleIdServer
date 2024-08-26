// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vp.Models;

public class InputDescriptorConstraints
{
    [JsonPropertyName("fields")]
    public List<InputDescriptorConstraintsField> Fields { get; set; }
}

public class InputDescriptorConstraintsField
{
    /// <summary>
    /// Be an array of one or more JSONPath string expressions
    /// </summary>
    [JsonPropertyName("path")]
    public List<string> Path { get; set; }
    /// <summary>
    /// Unique from every other field object's id property.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    /// <summary>
    /// Describes the purpose for which the field is being requested.
    /// </summary>
    [JsonPropertyName("purpose")]
    public string Purpose { get; set; }
    /// <summary>
    /// A human-friendly name that describes what the target field represents.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    /// <summary>
    /// JSON Schema descriptor used to filter against the values returned from evaluation of the JSONPath string expressions in the path array.
    /// </summary>
    [JsonPropertyName("filter")]
    public JsonObject Filter { get; set; }
}