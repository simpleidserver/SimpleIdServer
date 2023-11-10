// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class UpdateScopeRequest
{
    [JsonPropertyName(ScopeNames.Description)]
    public string Description { get; set; }
    [JsonPropertyName(ScopeNames.IsExposedInConfigurationEdp)]
    public bool IsExposedInConfigurationEdp { get; set; }
}
