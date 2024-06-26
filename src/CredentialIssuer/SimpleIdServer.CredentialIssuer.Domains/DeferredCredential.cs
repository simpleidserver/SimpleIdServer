// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class DeferredCredential
{
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; } = null!;
    [JsonPropertyName("credential_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CredentialId { get; set; } = null;
    [JsonPropertyName("credential_configuration_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CredentialConfigurationId { get; set; } = null;
    [JsonPropertyName("status")]
    public DeferredCredentialStatus Status { get; set; }
    [JsonPropertyName("formatter_name")]
    public string FormatterName { get; set; }
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; }
    [JsonPropertyName("encryption_alg")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncryptionAlg { get; set; } = null;
    [JsonPropertyName("encryption_enc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncryptionEnc { get; set; } = null;
    [JsonPropertyName("encryption_jwk")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncryptionJwk { get; set; } = null;
    [JsonPropertyName("create_datetime")]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName("configuration")]
    public CredentialConfiguration Configuration { get; set; }
    [JsonPropertyName("claims")]
    public List<DeferredCredentialClaim> Claims { get; set; }
}

public enum DeferredCredentialStatus
{
    PENDING = 0,
    ISSUED = 1
}