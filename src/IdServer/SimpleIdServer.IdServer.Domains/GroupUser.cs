// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class GroupUser
{
    [JsonIgnore]
    public string GroupsId { get; set; } = null!;
    [JsonIgnore]
    public string UsersId { get; set; } = null!;
    [JsonPropertyName("group")]
    public Group Group { get; set; }
    [JsonIgnore]
    public User User { get; set; }
}
