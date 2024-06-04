// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;
using SqlSugar.Extensions;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Translations")]
public class SugarTranslation
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Value { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Language { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? ClientId { get; set; } = null;

    public static SugarTranslation Transform(Translation translation)
    {
        return new SugarTranslation
        {
            Key = translation.Key,
            Language = translation.Language,
            Value = translation.Value
        };
    }

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
