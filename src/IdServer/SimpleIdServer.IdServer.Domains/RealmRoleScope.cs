// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains;

public class RealmRoleScope
{
    public string RealmRoleId { get; set; } = null!;
    public string ScopeId { get; set; } = null!;
    public RealmRole Role { get; set; } = null!;
    public Scope Scope { get; set; } = null!;
}
