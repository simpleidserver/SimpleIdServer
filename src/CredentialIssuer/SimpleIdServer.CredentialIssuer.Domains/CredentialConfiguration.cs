// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfiguration
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("server_id")]
    public string ServerId { get; set; }
    [JsonPropertyName("json_ld_context")]
    public string JsonLdContext { get; set; }
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
    [JsonPropertyName("create_datetime")]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName("update_datetime")]
    public DateTime UpdateDateTime { get; set; }
    [JsonIgnore]
    public string? CredentialSchemaId { get; set; } = null;
    [JsonIgnore]
    public string? CredentialSchemaType { get; set; } = null;
    [JsonPropertyName("claims")]
    public virtual List<CredentialConfigurationClaim> Claims { get; set; } = new List<CredentialConfigurationClaim>();
    [JsonPropertyName("displays")]
    public virtual List<CredentialConfigurationTranslation> Displays { get; set; } = new List<CredentialConfigurationTranslation>();
    [JsonIgnore]
    public List<string> AdditionalTypes { get; set; } = new List<string>();
    [JsonIgnore]
    public virtual List<Credential> Credentials { get; set; }
}