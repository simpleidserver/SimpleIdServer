// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class DeferredCredentialClaim
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
    [JsonIgnore]
    public string DeferredCredentialId { get; set; }
}
