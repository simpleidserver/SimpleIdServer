// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("DataProtectionKeys")]
public class SugarDataProtection
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? FriendlyName { get; set; }
    [SugarColumn(IsNullable = true, Length = 5000)]
    public string? Xml { get; set; }
}
