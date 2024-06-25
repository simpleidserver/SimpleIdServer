// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential;

public class CredentialResult
{
    [JsonPropertyName(CredentialResultNames.Format)]
    public string Format { get; set; }
    [JsonPropertyName(CredentialResultNames.Credential)]
    public JsonNode Credential { get; set; }
    [JsonPropertyName(CredentialResultNames.CNonce)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CNonce { get; set; }
    [JsonPropertyName(CredentialResultNames.CNonceExpiresIn)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CNonceExpiresIn { get; set; }
    [JsonPropertyName(CredentialResultNames.TransactionId)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TransactionId { get; set; }
}