// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Realms")]
public class SugarRealm
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "Name")]
    public string RealmsName { get; set; }
    public string Description { get; set; }
    [Navigate(typeof(SugarApiResourceRealm), nameof(SugarApiResourceRealm.RealmsName), nameof(SugarApiResourceRealm.ApiResourcesId))]
    public List<SugarApiResource> ApiResources { get; set; }
}