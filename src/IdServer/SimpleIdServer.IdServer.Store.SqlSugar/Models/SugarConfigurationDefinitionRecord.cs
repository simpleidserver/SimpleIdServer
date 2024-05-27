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
    public string? DisplayCondition { get; set; }
    [Navigate(typeof(SugarConfigurationDefinitionRecordTranslation), nameof(SugarConfigurationDefinitionRecordTranslation.ConfigurationDefinitionRecordId), nameof(SugarConfigurationDefinitionRecordTranslation.TranslationsId))]
    public List<SugarTranslation> Translations { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarConfigurationDefinitionRecordValue.ConfigurationDefinitionRecordId))]
    public List<SugarConfigurationDefinitionRecordValue> Values { get; set; }

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
