// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class AuthenticationProfileResult
{
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}