// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.AuthFaker.VpRegistration;

public class BeginU2FRegisterResult
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = null!;
    [JsonPropertyName("login")]
    public string Login { get; set; } = null!;
    [JsonPropertyName("credential_create_options")]
    public CredentialCreateOptions CredentialCreateOptions { get; set; } = null!;
    [JsonPropertyName("end_register_url")]
    public string EndRegisterUrl { get; set; } = null!;
}
