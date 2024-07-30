// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class RealmRole
{
    [JsonPropertyName(RealmRoleNames.Id)]
    public string Id { get; set; } = null!;
    [JsonPropertyName(RealmRoleNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(RealmRoleNames.Description)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; } = null;
    [JsonPropertyName(RealmRoleNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(RealmRoleNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(RealmRoleNames.Realm)]
    public string RealmName { get; set; } = null!;
    [JsonIgnore]
    public Realm Realm { get; set; } = null!;
    [JsonPropertyName(RealmRoleNames.Scopes)]
    public List<RealmRoleScope> Scopes { get; set; }
}