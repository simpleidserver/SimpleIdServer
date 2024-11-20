// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class Credential
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("credential_id")]
    public string CredentialId { get; set; }
    [JsonPropertyName("sub")]
    public string Subject { get; set; }
    [JsonPropertyName("credential_configuration_id")]
    public string CredentialConfigurationId { get; set; }
    [JsonPropertyName("issue_datetime")]
    public DateTime? IssueDateTime { get; set; }
    [JsonPropertyName("exp_datetime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ExpirationDateTime { get; set; }
    [JsonIgnore]
    public virtual CredentialConfiguration Configuration { get; set; }
    [JsonPropertyName("claims")]
    public virtual List<CredentialClaim> Claims { get; set; }
    public bool IsDeferred { get; set; } = false;
}
