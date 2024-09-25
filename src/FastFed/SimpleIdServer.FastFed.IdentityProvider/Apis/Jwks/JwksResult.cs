// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.Jwks;

public class JwksResult
{
    [JsonPropertyName("keys")]
    public List<JsonObject> JsonWebKeys { get; set; } = new List<JsonObject>();
}
