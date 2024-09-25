// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Jwks
{
    public class JwksResult
    {
        [JsonPropertyName("keys")]
        public ICollection<JsonObject> JsonWebKeys { get; set; } = new List<JsonObject>();
    }
}