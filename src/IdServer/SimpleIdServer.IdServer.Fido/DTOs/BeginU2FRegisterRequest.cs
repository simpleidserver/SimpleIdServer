// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class BeginU2FRegisterRequest
    {
        [JsonPropertyName(BeginU2FRegisterRequestNames.Login)]
        public string? Login { get; set; } = null;
        [JsonPropertyName(BeginU2FRegisterRequestNames.DisplayName)]
        public string DisplayName { get; set; } = null!;
        [JsonPropertyName(BeginU2FRegisterRequestNames.CredentialType)]
        public string CredentialType { get; set; } = null!;
    }
}
