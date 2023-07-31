// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class BeginU2FLoginResult
    {
        [JsonPropertyName(BeginU2FLoginResultNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(BeginU2FLoginResultNames.Assertion)]
        public AssertionOptions Assertion { get; set; } = null!;
    }
}
