// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Translations")]
public class SugarTranslation
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string? Value { get; set; } = null!;
    public string? Language { get; set; } = null;
    public string ClientId { get; set; } = null;

    public Translation ToDomain()
    {
        return new Translation
        {
            Key = Key,
            Value = Value,
            Language = Language
        };
    }
}
