// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Apis.FederationFetch;

public class FederationFetchRequest
{
    /// <summary>
    /// The Entity Identifier of the issuer from which the Entity Statement is issued.
    /// </summary>
    [JsonPropertyName("iss")]
    public string Iss { get; set; } = null!;
    [JsonPropertyName("sub")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Sub { get; set; } = null;
}
