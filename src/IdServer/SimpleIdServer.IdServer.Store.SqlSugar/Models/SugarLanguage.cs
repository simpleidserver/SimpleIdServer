// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Languages")]
public class SugarLanguage
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Code { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }

    public static SugarLanguage Transform(Language language)
    {
        return new SugarLanguage
        {
            Code = language.Code,
            CreateDateTime = language.CreateDateTime,
            UpdateDateTime = language.UpdateDateTime
        };
    }

    public Language ToDomain()
    {
        return new Language
        {
            Code = Code,
            CreateDateTime = CreateDateTime,
            UpdateDateTime  = UpdateDateTime
        };
    }
}
