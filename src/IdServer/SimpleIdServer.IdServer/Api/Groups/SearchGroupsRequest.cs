// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.IdServer.DTOs;

namespace SimpleIdServer.IdServer.Api.Groups;

public class SearchGroupsRequest : SearchRequest
{
    [JsonProperty("only_root")]
    public bool OnlyRoot { get; set; }
}
