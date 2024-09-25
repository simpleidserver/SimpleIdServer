// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.ApplicationProvider.Services;

public class JwksResult
{
    [JsonPropertyName("keys")]
    public List<JsonWebKey> Keys { get; set; }
}
