// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.AuthenticationClassReferences;

public class AddAuthenticationClassReferenceRequest
{
    [JsonPropertyName(AuthenticationContextClassReferenceNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(AuthenticationContextClassReferenceNames.DisplayName)]
    public string DisplayName { get; set; }
    [JsonPropertyName(AuthenticationContextClassReferenceNames.AuthenticationMethodReferences)]
    public IEnumerable<string> AuthenticationMethodReferences { get; set; }
}
