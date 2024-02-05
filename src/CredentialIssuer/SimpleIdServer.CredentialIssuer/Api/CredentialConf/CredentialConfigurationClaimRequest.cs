// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

public class CredentialConfigurationClaimRequest
{

    [JsonPropertyName("source_claim_name")]
    public string SourceUserClaimName { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }
    [JsonPropertyName("value_type")]
    public string? ValueType { get; set; }
}
