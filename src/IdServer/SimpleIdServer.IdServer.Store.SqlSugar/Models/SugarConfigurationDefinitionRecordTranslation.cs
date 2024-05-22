// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ConfigurationDefinitionRecordTranslation")]
public class SugarConfigurationDefinitionRecordTranslation
{
    [SugarColumn(IsPrimaryKey = true)]
    public string ConfigurationDefinitionRecordId { get; set; } = null!;
    [SugarColumn(IsPrimaryKey = true)]
    public string TranslationsId { get; set; } = null!;
}
