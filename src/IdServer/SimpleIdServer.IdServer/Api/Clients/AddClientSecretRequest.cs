// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class AddClientSecretRequest
{
    [JsonPropertyName(ClientSecretNames.Value)]
    public string Value { get; set; }
    [JsonPropertyName(ClientSecretNames.Alg)]
    public HashAlgs Alg { get; set; }
}
