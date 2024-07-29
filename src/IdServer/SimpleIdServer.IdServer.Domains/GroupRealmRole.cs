// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains;

public class GroupRealmRole
{
    public string GroupId { get; set; } = null!;
    public string RealmRoleId { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public RealmRole RealmRole { get; set; } = null!;
}
