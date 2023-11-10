// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Groups;

public class GetGroupResult
{
    [JsonPropertyName("target")]
    public Group Target { get; set; }
    [JsonPropertyName("root")]
    public Group Root { get; set; }
}
