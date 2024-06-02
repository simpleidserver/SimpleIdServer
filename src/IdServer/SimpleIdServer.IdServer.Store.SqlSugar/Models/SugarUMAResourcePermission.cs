// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UMAResourcePermission")]
public class SugarUMAResourcePermission
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public string Scopes { get; set; } = null!;
    public string UMAResourceId { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUMAResourcePermissionClaim.UMAResourcePermissionId))]
    public List<SugarUMAResourcePermissionClaim> Claims { get; set; }

    public UMAResourcePermission ToDomain()
    {
        return new UMAResourcePermission
        {
            Id = Id,
            CreateDateTime = CreateDateTime,
            Scopes = Scopes.Split(',').ToList(),
            Claims = Claims.Select(c => c.ToDomain()).ToList()
        };
    }
}
