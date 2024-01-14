// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class GroupRealm
{
    public string RealmsName { get; set; } = null!;
    public string GroupsId { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public Realm Realm { get; set; } = null!;
}
