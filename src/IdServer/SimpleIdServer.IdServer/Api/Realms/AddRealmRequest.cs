// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Realms;

public class AddRealmRequest
{
    [JsonPropertyName(RealmNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(RealmNames.Description)]
    public string Description { get; set; }
}
