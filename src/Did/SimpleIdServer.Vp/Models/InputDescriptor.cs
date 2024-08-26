// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vp.Models;

public class InputDescriptor
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    /// <summary>
    /// Human-friendly string intended to constitute a distinctive designation of the Presentation Definition.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    /// <summary>
    /// Describes the purpose for which the Presentation Definition's inputs are being used for.
    /// </summary>
    [JsonPropertyName("purpose")]
    public string Purpose { get; set; }
    /// <summary>
    /// Must be an object  with one or more properties matching the registered Claim Format Designations (e.g., jwt, jwt_vc, jwt_vp, etc.). 
    /// The properties inform the Holder of the Claim format configurations the Verifier can process. 
    /// </summary>
    [JsonPropertyName("format")]
    public Dictionary<string, InputDescriptorFormat> Format { get; set; }
    [JsonPropertyName("constraints")]
    public InputDescriptorConstraints Constraints { get; set; }
}