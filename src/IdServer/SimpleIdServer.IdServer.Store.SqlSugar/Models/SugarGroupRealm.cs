// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("GroupRealm")]
public class SugarGroupRealm
{
    [SugarColumn(IsPrimaryKey = true)]
    public string GroupsId { get; set; } = null!;
    [SugarColumn(IsPrimaryKey = true)]
    public string RealmsName { get; set; } = null!;

    public GroupRealm ToDomain()
    {
        return new GroupRealm
        {
            GroupsId = GroupsId,
            RealmsName = RealmsName
        };
    }
}