// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.Domains
{
    public class ClientJsonWebKey
    {
        public string Kid { get; set; } = null!;
        public string SerializedJsonWebKey { get; set; } = null!;

        public JsonObject Serialize() => JsonNode.Parse(SerializedJsonWebKey).AsObject();
    }
}
