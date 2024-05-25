// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("GroupUser")]
public class SugarGroupUser
{
    [SugarColumn(IsPrimaryKey = true)]
    public string GroupsId { get; set; } = null!;
    [SugarColumn(IsPrimaryKey = true)]
    public string UsersId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(GroupsId))]
    public SugarGroup Group { get; set; }

    public GroupUser ToDomain()
    {
        return new GroupUser
        {
            GroupsId = GroupsId,
            UsersId = UsersId,
            Group = Group.ToDomain()
        };
    }
}
