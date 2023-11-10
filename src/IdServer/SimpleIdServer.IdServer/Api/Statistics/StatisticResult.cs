// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Statistics;

public class StatisticResult
{
    [JsonPropertyName("clients")]
    public int NbClients { get; set; }
    [JsonPropertyName("users")]
    public int NbUsers { get; set; }
    [JsonPropertyName("invalid_authentications")]
    public int InvalidAuthentications { get; set; }
    [JsonPropertyName("valid_authentications")]
    public int ValidAuthentications { get; set; }
}
