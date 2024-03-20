// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class PresentationDefinition
{
    [JsonIgnore]
    public string Id { get; set; } = null!;
    /// <summary>
    /// The string SHOULD provide a unique ID for the desired context.
    /// </summary>
    [JsonPropertyName("id")]
    public string PublicId { get; set; } = null!;
    /// <summary>
    /// If present, its value SHOULD be a human-friendly string intended to constitute a distinctive designation of the Presentation Definition.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; } = null;
    /// <summary>
    /// If present, its value MUST be a string that describes the purpose for which the Presentation Definition's inputs are being used for.
    /// </summary>
    [JsonPropertyName("purpose")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Purpose { get; set; } = null;
    [JsonPropertyName("input_descriptors")]
    public List<PresentationDefinitionInputDescriptor> InputDescriptors { get; set; } = new List<PresentationDefinitionInputDescriptor>();
    [JsonIgnore]
    public string RealmName { get; set; } = null!;
    [JsonIgnore]
    public Realm Realm { get; set; }
}