// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users
{
    public class GenerateDecentralizedIdentifierResult
    {
        [JsonPropertyName(GenerateDecentralizedIdentifierResultNames.DID)]
        public string DID { get; set; } = null!;
        [JsonPropertyName(GenerateDecentralizedIdentifierResultNames.PrivateKey)]
        public string PrivateKey { get; set; } = null!;
    }
}
