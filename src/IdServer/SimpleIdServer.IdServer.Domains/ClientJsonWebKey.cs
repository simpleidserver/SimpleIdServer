// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains
{
    public class ClientJsonWebKey
    {
        public string Kid { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public string Usage { get; set; } = null!;
        public SecurityKeyTypes? KeyType { get; set; } = null;
        public string SerializedJsonWebKey { get; set; } = null!;

        public JsonObject Serialize() => JsonNode.Parse(SerializedJsonWebKey).AsObject();
    }

    public enum SecurityKeyTypes
    {
        RSA = 0,
        CERTIFICATE = 1,
        ECDSA = 2
    }
}
