// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class UpdateScopeResourcesRequest
{
    [JsonPropertyName(ScopeNames.Resources)]
    public IEnumerable<string> Resources { get; set; }
}
