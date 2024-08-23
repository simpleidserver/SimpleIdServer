// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vp.Models;

public class VerifiablePresentation : BaseVerifiableDocument
{
    /// <summary>
    /// The id property is optional and MAY be used to provide a unique identifier for the presentation. 
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; } = null;
    [JsonPropertyName("holder")]
    public string Holder { get; set; }
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new List<string>();
    /// <summary>
    /// The type property is required and expresses the type of presentation, such as VerifiablePresentation.
    /// </summary>
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new List<string>();
    [JsonPropertyName("verifiableCredential")]
    public List<string> VerifiableCredential { get; set; } = new List<string>();

    public Dictionary<string, object> ToDic()
    {
        var result = new Dictionary<string, object>
        {
            { "@context", Context },
            { "id", Id },
            { "type", Type },
            { "holder", Holder },
            { "verifiableCredential", VerifiableCredential }
        };
        return result;
    }
}