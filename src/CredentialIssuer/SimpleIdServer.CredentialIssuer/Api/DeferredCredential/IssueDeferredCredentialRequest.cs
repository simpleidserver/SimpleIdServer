// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.DeferredCredential;

public class IssueDeferredCredentialRequest
{
    [JsonPropertyName(DeferredCredentialResultNames.Claims)]
    public Dictionary<string, string> Claims { get; set; }
}
