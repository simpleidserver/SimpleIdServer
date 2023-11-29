// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class UpdateScopeClaimRequest
{
    [JsonPropertyName(ScopeClaimMapperNames.SourceUserAttribute)]
    public string? SourceUserAttribute { get; set; } = null;
    [JsonPropertyName(ScopeClaimMapperNames.SourceUserProperty)]
    public string? SourceUserProperty { get; set; } = null;
    [JsonPropertyName(ScopeClaimMapperNames.TargetClaimPath)]
    public string? TargetClaimPath { get; set; } = null;
    [JsonPropertyName(ScopeClaimMapperNames.SAMLAttributeName)]
    public string? SAMLAttributeName { get; set; } = null;
    [JsonPropertyName(ScopeClaimMapperNames.TokenClaimJsonType)]
    public TokenClaimJsonTypes? TokenClaimJsonType { get; set; } = null;
    [JsonPropertyName(ScopeClaimMapperNames.IsMultivalued)]
    public bool IsMultiValued { get; set; } = false;
    [JsonPropertyName(ScopeClaimMapperNames.IncludeInAccessToken)]
    public bool IncludeInAccessToken { get; set; } = false;
}
