// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients
{
    public class GenerateEncKeyRequest
    {
        [JsonPropertyName(ClientJsonWebKeyNames.Kid)]
        public string KeyId { get; set; } = null!;
        [JsonPropertyName(ClientJsonWebKeyNames.KeyType)]
        public SecurityKeyTypes KeyType { get; set; }
        [JsonPropertyName(ClientJsonWebKeyNames.Alg)]
        public string Alg { get; set; } = null!;
        [JsonPropertyName(ClientJsonWebKeyNames.Enc)]
        public string Enc { get; set; } = null!;
    }
}