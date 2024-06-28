// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential;

public class ESBICredentialResult
{
    [JsonPropertyName(CredentialResultNames.AcceptanceToken)]
    public string AcceptanceToken { get; set; }
    [JsonPropertyName(CredentialResultNames.CNonce)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CNonce { get; set; }
}
