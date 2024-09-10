// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class IdentityProviderMetadata : BaseProviderMetadata
{
    /// <summary>
    /// URL of the Identity Provider's JSON Web Key Set [JWK] containing the signing key(s) used by the Provider.
    /// </summary>
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = null!;
    /// <summary>
    /// A URL which specifies the endpoint for the Identity Provider to receive a FastFed Handshake initiation request.
    /// </summary>
    [JsonPropertyName("fastfed_handshake_start_uri")]
    public string FastFedHandshakeStartUri { get; set; } = null!;

    protected override List<string> InternalValidate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(JwksUri)) result.Add(string.Empty);
        if (string.IsNullOrWhiteSpace(FastFedHandshakeStartUri)) result.Add(string.Empty);
        return result;
    }
}
