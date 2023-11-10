// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Groups;

public class AddGroupRequest
{
    [JsonPropertyName(GroupNames.ParentGroupId)]
    public string ParentGroupId { get; set; }
    [JsonPropertyName(GroupNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(GroupNames.Description)]
    public string Description { get; set; }
}
