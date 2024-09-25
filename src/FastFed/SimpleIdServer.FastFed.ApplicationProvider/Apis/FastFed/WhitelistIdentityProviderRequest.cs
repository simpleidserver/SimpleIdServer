// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.FastFed;

public class WhitelistIdentityProviderRequest
{
    [JsonPropertyName("identity_provider_url")]
    public string IdentityProviderUrl { get; set; } = null;
}
