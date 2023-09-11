// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

public class AddAuthenticationSchemeProviderRequest
{
    [JsonPropertyName(AuthenticationSchemeProviderNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.Description)]
    public string Description { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.DisplayName)]
    public string DisplayName { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.DefinitionName)]
    public string DefinitionName { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.Values)]
    public Dictionary<string, string> Values { get; set; }
}
