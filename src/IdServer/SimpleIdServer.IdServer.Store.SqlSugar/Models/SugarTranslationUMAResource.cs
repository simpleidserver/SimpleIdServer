// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("TranslationUMAResource")]
public class SugarTranslationUMAResource
{
    [SugarColumn(IsPrimaryKey = true)]
    public int TranslationsId { get; set; }
    [SugarColumn(IsPrimaryKey = true)]
    public string UMAResourceId { get; set; }
}
