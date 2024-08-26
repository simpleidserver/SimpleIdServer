// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vp.Models;

public class InputDescriptorFormat
{
    [JsonPropertyName("alg")]
    public List<string> Alg { get; set; }
    [JsonPropertyName("proof_type")]
    public List<string> ProofType { get; set; }
}