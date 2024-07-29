// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class UpdateClientRealmsRequest
{
    [JsonPropertyName(OAuthClientParameters.Realms)]
    public List<string> Realms { get; set; } = new List<string>();
}
