// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class RealmRoleScope
{
    [JsonIgnore]
    public string RealmRoleId { get; set; } = null!;
    [JsonIgnore]
    public string ScopeId { get; set; } = null!;
    [JsonIgnore]
    public RealmRole Role { get; set; } = null!;
    public Scope Scope { get; set; } = null!;
}
