// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class EndU2FRegisterRequest
    {
        [JsonPropertyName(EndU2FRegisterRequestNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(EndU2FRegisterRequestNames.Login)]
        public string? Login { get; set; } = null;
        [JsonPropertyName(EndU2FRegisterRequestNames.AuthenticatorAttestationRawResponse)]
        public AuthenticatorAttestationRawResponse AuthenticatorAttestationRawResponse { get; set; } = null!;
        [JsonPropertyName(EndU2FRegisterRequestNames.DeviceData)]
        public EndU2FRegisterDeviceRequest DeviceData { get; set; } = null!;
    }
}
