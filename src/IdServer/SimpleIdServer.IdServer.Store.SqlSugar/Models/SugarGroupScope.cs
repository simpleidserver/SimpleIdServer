// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("GroupScope")]
public class SugarGroupScope
{
    [SugarColumn(IsPrimaryKey = true)]
    public string GroupsId { get; set; } = null!;
    [SugarColumn(IsPrimaryKey = true)]
    public string RolesId { get; set; } = null!;
}
