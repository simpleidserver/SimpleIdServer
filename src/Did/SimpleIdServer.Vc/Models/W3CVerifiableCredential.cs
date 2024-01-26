// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models;

/// <summary>
/// https://www.w3.org/2018/credentials/#sotd
/// </summary>
public class W3CVerifiableCredential
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new List<string>();
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new List<string>
    {
        "VerifiableCredential"
    };
    [JsonPropertyName("issuer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Issuer { get; set; }
    [JsonPropertyName("validFrom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidFrom { get; set; }
    [JsonPropertyName("validUntil")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidUntil { get; set; }
    [JsonPropertyName("credentialSubject")]
    public JsonObject CredentialSubject { get; set; }
}