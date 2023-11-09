// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class AddEncKeyRequest
{
    [JsonPropertyName(ClientJsonWebKeyNames.Kid)]
    public string KeyId { get; set; }
    [JsonPropertyName(ClientJsonWebKeyNames.KeyType)]
    public SecurityKeyTypes KeyType { get; set; }
    [JsonPropertyName(ClientJsonWebKeyNames.SerializedJwk)]
    public string SerializedJsonWebKey { get; set; }
    [JsonPropertyName(ClientJsonWebKeyNames.Alg)]
    public string Alg { get; set; }
}
