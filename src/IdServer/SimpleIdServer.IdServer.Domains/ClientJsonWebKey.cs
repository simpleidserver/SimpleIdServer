// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class ClientJsonWebKey
    {
        [JsonPropertyName(ClientJsonWebKeyNames.Kid)]
        public string Kid { get; set; } = null!;
        [JsonPropertyName(ClientJsonWebKeyNames.Alg)]
        public string Alg { get; set; } = null!;
        [JsonPropertyName(ClientJsonWebKeyNames.Usage)]
        public string Usage { get; set; } = null!;
        [JsonPropertyName(ClientJsonWebKeyNames.KeyType)]
        public SecurityKeyTypes? KeyType { get; set; } = null;
        [JsonPropertyName(ClientJsonWebKeyNames.SerializedJwk)]
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
