// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class BeginU2FRegisterResult
    {
        [JsonPropertyName(BeginU2FRegisterResultNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(BeginU2FRegisterResultNames.CredentialCreateOptions)]
        public CredentialCreateOptions CredentialCreateOptions { get; set; } = null!;
        [JsonPropertyName(BeginU2FRegisterResultNames.EndRegisterUrl)]
        public string EndRegisterUrl { get; set; } = null!;
        [JsonPropertyName(BeginU2FRegisterResultNames.Login)]
        public string Login { get; set; } = null!;
    }
}
