// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

public class CredentialConfigurationClaimDisplayRequest
{
    [JsonPropertyName("locale")]
    public string Locale { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
