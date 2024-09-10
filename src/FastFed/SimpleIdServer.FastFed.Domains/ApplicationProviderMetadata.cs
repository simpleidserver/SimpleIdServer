// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class ApplicationProviderMetadata : BaseProviderMetadata
{
    /// <summary>
    /// A URL which specifies the FastFed Handshake registration endpoint.
    /// </summary>
    [JsonPropertyName("fastfed_handshake_register_uri")]
    public string FastfedHandshakeRegisterUri { get; set; } = null!;

    protected override List<string> InternalValidate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(FastfedHandshakeRegisterUri)) result.Add(string.Empty);
        return result;
    }
}
