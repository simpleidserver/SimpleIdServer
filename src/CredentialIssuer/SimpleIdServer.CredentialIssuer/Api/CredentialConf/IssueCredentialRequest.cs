// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

public class IssueCredentialRequest
{
    [JsonPropertyName("issue_datetime")]
    public DateTime IssueDateTime { get; set; }
    [JsonPropertyName("expiration_datetime")]
    public DateTime? ExpirationDateTime { get; set; }
    [JsonPropertyName("sub")]
    public string Subject { get; set; }
    [JsonPropertyName("credential_id")]
    public string CredentialId { get; set; }
}
