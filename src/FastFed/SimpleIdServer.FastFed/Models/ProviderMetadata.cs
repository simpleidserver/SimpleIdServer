// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Models;

public class ProviderMetadata
{
    /// <summary>
    /// The existence of this element indicates the entity is capable of acting as an Identity Provider.
    /// </summary>
    [JsonPropertyName("identity_provider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IdentityProviderMetadata IdentityProvider { get; set; } = null;
    /// <summary>
    /// The existence of this element indicates the entity is capable of acting as an Application Provider.
    /// </summary>
    [JsonPropertyName("application_provider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApplicationProviderMetadata ApplicationProvider { get; set; } = null;
}
