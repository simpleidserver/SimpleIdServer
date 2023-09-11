// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

public class AuthenticationSchemeProviderResult
{
    [JsonPropertyName(AuthenticationSchemeProviderNames.Id)]
    public string Id { get; set; } = null!;
    [JsonPropertyName(AuthenticationSchemeProviderNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(AuthenticationSchemeProviderNames.DisplayName)]
    public string? DisplayName { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.Description)]
    public string? Description { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.Mappers)]
    public ICollection<AuthenticationSchemeProviderMapperResult> Mappers { get; set; } = new List<AuthenticationSchemeProviderMapperResult>();
    [JsonPropertyName(AuthenticationSchemeProviderNames.Definition)]
    public AuthenticationSchemeProviderDefinitionResult Definition { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderNames.Values)]
    public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
}