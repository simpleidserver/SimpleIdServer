// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.ApiResources;

public class AddApiResourceRequest
{
    [JsonPropertyName(ApiResourceNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(ApiResourceNames.Audience)]
    public string? Audience { get; set; } = null;
    [JsonPropertyName(ApiResourceNames.Description)]
    public string? Description { get; set; } = null;
}