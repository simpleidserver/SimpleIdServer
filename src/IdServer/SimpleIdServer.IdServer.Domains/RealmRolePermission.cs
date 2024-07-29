// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class RealmRolePermission
{
    [JsonPropertyName(RealmRolePermissionNames.Id)]
    public string Id { get; set; } = null!;
    [JsonPropertyName(RealmRolePermissionNames.Component)]
    public string Component { get; set; } = null!;
    [JsonPropertyName(RealmRolePermissionNames.PossibleActions)]
    public ActionTypes PossibleActions { get; set; }
    [JsonIgnore]
    public string RoleName { get; set; } = null!;
    [JsonIgnore]
    public RealmRole Role { get; set; } = null!;
}

[Flags]
public enum ActionTypes
{
    View = 1,
    Manage = 2
}