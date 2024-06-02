// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UMAResourcePermissionClaim")]
public class SugarUMAResourcePermissionClaim
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string? ClaimType { get; set; } = null!;
    public string? FriendlyName { get; set; } = null;
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? UMAResourcePermissionId { get; set; } = null;

    public UMAResourcePermissionClaim ToDomain()
    {
        return new UMAResourcePermissionClaim
        {
            ClaimType = ClaimType,
            FriendlyName = FriendlyName,
            Value = Value,
            Name = Name
        };
    }
}
