// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vp.Models;

public class VerifiablePresentationDefinition
{
    /// <summary>
    /// Unique ID of the desired context.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    /// <summary>
    /// All inputs listed in the array are required for submission.
    /// </summary>
    [JsonPropertyName("input_descriptors")]
    public List<InputDescriptor> InputDescriptors { get; set; } = new List<InputDescriptor>();
}