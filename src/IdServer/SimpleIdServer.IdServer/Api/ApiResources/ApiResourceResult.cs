// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.ApiResources;

public class ApiResourceResult
{
    [JsonPropertyName(ApiResourceNames.Id)]
    public string Id { get; set; }
    [JsonPropertyName(ApiResourceNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(ApiResourceNames.Audience)]
    public string? Audience { get; set; } = null;
    [JsonPropertyName(ApiResourceNames.Description)]
    public string? Description { get; set; } = null;
    [JsonPropertyName(ApiResourceNames.CreateDatetime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(ApiResourceNames.UpdateDatetime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(ApiResourceNames.Scopes)]
    public IEnumerable<string> Scopes { get; set; }
}
