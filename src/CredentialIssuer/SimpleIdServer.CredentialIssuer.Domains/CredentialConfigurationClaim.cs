// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfigurationClaim
{
    [JsonPropertyName("id")]
    public string Id {  get; set; }
    [JsonPropertyName("source_claim_name")]
    public string SourceUserClaimName { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }
    [JsonPropertyName("value_type")]
    public string? ValueType { get; set; }
    [JsonPropertyName("translations")]
    public virtual List<CredentialConfigurationTranslation> Translations { get; set; } = new List<CredentialConfigurationTranslation>();
    [JsonIgnore]
    public virtual CredentialConfiguration CredentialConfiguration { get; set; }
}