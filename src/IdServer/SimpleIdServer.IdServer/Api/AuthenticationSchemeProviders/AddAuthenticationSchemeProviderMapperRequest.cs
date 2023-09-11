// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

public class AddAuthenticationSchemeProviderMapperRequest
{
    [JsonPropertyName(AuthenticationSchemeProviderMapperNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(AuthenticationSchemeProviderMapperNames.SourceClaimName)]
    public string? SourceClaimName { get; set; } = null;
    [JsonPropertyName(AuthenticationSchemeProviderMapperNames.MapperType)]
    public MappingRuleTypes MapperType { get; set; }
    [JsonPropertyName(AuthenticationSchemeProviderMapperNames.TargetUserAttribute)]
    public string? TargetUserAttribute { get; set; } = null;
    [JsonPropertyName(AuthenticationSchemeProviderMapperNames.TargetUserProperty)]
    public string? TargetUserProperty { get; set; } = null;
}
