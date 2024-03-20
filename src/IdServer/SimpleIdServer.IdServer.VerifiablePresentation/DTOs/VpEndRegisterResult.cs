// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

public class VpEndRegisterResult
{
    [JsonPropertyName("next_registration_url")]
    public string NextRegistrationRedirectUrl { get; set; }
}
