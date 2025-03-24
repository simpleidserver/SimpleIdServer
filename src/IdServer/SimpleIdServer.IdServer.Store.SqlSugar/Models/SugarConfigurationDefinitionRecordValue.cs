// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Helpers.Models;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ConfigurationDefinitionRecordValue")]
public class SugarConfigurationDefinitionRecordValue
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string ConfigurationDefinitionRecordId { get; set; }
    [Navigate(typeof(SugarConfigurationDefinitionRecordValueTranslation), nameof(SugarConfigurationDefinitionRecordValueTranslation.ConfigurationDefinitionRecordValueId), nameof(SugarConfigurationDefinitionRecordValueTranslation.TranslationsId))]
    public List<SugarTranslation> Translations { get; set; }

    public static SugarConfigurationDefinitionRecordValue Transform(ConfigurationDefinitionRecordValue value)
    {
        return new SugarConfigurationDefinitionRecordValue
        {
            Id = value.Id,
            Value = value.Value,
            Translations = value.Translations == null ? new List<SugarTranslation>() : value.Translations.Select(t => SugarTranslation.Transform(t)).ToList()
        };
    }

    public ConfigurationDefinitionRecordValue ToDomain()
    {
        return new ConfigurationDefinitionRecordValue
        {
            Id = Id,
            Value = Value,
            Translations = Translations == null ? new List<Translation>() : Translations.Select(t => t.ToDomain()).ToList()
        };
    }
}