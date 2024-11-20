﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class SearchScopeRequest : SearchRequest
{
    [JsonPropertyName(ScopeNames.Protocols)]
    public IEnumerable<ScopeProtocols> Protocols { get; set; }
    [JsonPropertyName(ScopeNames.IsRole)]
    public bool IsRole { get; set; }
}
