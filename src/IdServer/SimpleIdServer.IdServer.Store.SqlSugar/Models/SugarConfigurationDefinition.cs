// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration.Models;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Definitions")]
public class SugarConfigurationDefinition
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string FullQualifiedName { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarConfigurationDefinitionRecord.ConfigurationDefinitionId))]
    public List<SugarConfigurationDefinitionRecord> ConfigurationDefinitionRecords { get; set; }

    public static SugarConfigurationDefinition Transform(ConfigurationDefinition configurationDefinition)
    {
        return new SugarConfigurationDefinition
        {
            Id = configurationDefinition.Id,
            CreateDateTime = configurationDefinition.CreateDateTime,
            UpdateDateTime = configurationDefinition.UpdateDateTime,
            FullQualifiedName = configurationDefinition.FullQualifiedName,
            ConfigurationDefinitionRecords = configurationDefinition.Records == null ? new List<SugarConfigurationDefinitionRecord>() : configurationDefinition.Records.Select(r => SugarConfigurationDefinitionRecord.Transform(r)).ToList()
        };
    }

    public ConfigurationDefinition ToDomain()
    {
        return new ConfigurationDefinition
        {
            Id = Id,
            CreateDateTime  = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            FullQualifiedName = FullQualifiedName,
            Records = ConfigurationDefinitionRecords == null ? new List<ConfigurationDefinitionRecord>() : ConfigurationDefinitionRecords.Select(r => r.ToDomain()).ToList()
        };
    }
}
