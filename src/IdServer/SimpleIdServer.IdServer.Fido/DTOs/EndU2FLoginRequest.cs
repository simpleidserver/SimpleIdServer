// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class EndU2FLoginRequest
    {
        [JsonPropertyName(EndU2FLoginRequestNames.Login)]
        public string? Login { get; set; } = null;
        [JsonPropertyName(EndU2FLoginRequestNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(EndU2FLoginRequestNames.Assertion)]
        public AuthenticatorAssertionRawResponse Assertion { get; set; } = null!;
    }
}
