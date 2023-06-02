// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users
{
    [JsonConverter(typeof(GenerateDecentralizedIdentityRequestConverter))]
    public class GenerateDecentralizedIdentityRequest
    {
        [JsonPropertyName(GenerateDecentralizedIdentityRequestNames.Method)]
        public string Method { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
