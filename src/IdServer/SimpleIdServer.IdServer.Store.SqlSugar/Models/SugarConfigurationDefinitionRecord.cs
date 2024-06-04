// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ConfigurationDefinitionRecord")]
public class SugarConfigurationDefinitionRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ConfigurationDefinitionId { get; set; } = null;
    public bool IsRequired { get; set; }
    public ConfigurationDefinitionRecordTypes Type { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public int Order { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? DisplayCondition { get; set; }
    [Navigate(typeof(SugarConfigurationDefinitionRecordTranslation), nameof(SugarConfigurationDefinitionRecordTranslation.ConfigurationDefinitionRecordId), nameof(SugarConfigurationDefinitionRecordTranslation.TranslationsId))]
    public List<SugarTranslation> Translations { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarConfigurationDefinitionRecordValue.ConfigurationDefinitionRecordId))]
    public List<SugarConfigurationDefinitionRecordValue> Values { get; set; }

    public static SugarConfigurationDefinitionRecord Transform(ConfigurationDefinitionRecord record)
    {
        return new SugarConfigurationDefinitionRecord
        {
            Id = record.Id,
            Name = record.Name,
            IsRequired = record.IsRequired,
            Type = record.Type,
            CreateDateTime = record.CreateDateTime,
            UpdateDateTime = record.UpdateDateTime,
            Order = record.Order,
            DisplayCondition = record.DisplayCondition,
            Translations = record.Translations == null ? new List<SugarTranslation>() : record.Translations.Select(t => SugarTranslation.Transform(t)).ToList(),
            Values = record.Values == null ? new List<SugarConfigurationDefinitionRecordValue>() : record.Values.Select(v => SugarConfigurationDefinitionRecordValue.Transform(v)).ToList()
        };
    }

    public ConfigurationDefinitionRecord ToDomain()
    {
        return new ConfigurationDefinitionRecord
        {
            Id = Id,
            Name = Name,
            Type = Type,
            CreateDateTime = CreateDateTime,
            DisplayCondition = DisplayCondition,
            IsRequired = IsRequired,
            Order = Order,
            UpdateDateTime = UpdateDateTime,
            Translations = Translations == null ? new List<Translation>() : Translations.Select(t => t.ToDomain()).ToList(),
            Values = Values == null ? new List<ConfigurationDefinitionRecordValue>() : Values.Select(t => t.ToDomain()).ToList()
        };
    }
}
