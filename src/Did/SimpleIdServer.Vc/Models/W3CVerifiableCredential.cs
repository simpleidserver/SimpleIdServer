// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models;

/// <summary>
/// https://www.w3.org/2018/credentials/#sotd
/// </summary>
public class W3CVerifiableCredential : BaseVerifiableDocument
{
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new List<string>();
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new List<string>();
    [JsonPropertyName("issuer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Issuer { get; set; }
    [JsonPropertyName("issuanceDate")]
    public DateTime IssuanceDate { get; set; }
    [JsonPropertyName("validFrom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidFrom { get; set; }
    [JsonPropertyName("validUntil")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidUntil { get; set; }
    [JsonPropertyName("issued")]
    public DateTime Issued { get; set; }
    [JsonPropertyName("credentialSubject")]
    public JsonNode CredentialSubject { get; set; }
    [JsonPropertyName("credentialSchema")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public W3CVerifiableCredentialSchema Schema { get; set; }
}

public class W3CVerifiableCredentialSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
}