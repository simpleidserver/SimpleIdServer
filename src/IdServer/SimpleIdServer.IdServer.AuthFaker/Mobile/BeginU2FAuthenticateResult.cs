// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.AuthFaker.Mobile;

public class BeginU2FAuthenticateResult
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = null!;
    [JsonPropertyName("login")]
    public string Login { get; set; } = null!;
    [JsonPropertyName("assertion")]
    public AssertionOptions Assertion { get; set; } = null!;
    [JsonPropertyName("end_login_url")]
    public string EndLoginUrl { get; set; } = null!;
}