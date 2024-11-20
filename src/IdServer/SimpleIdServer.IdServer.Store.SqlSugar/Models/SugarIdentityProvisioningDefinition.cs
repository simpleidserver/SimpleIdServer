﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("IdentityProvisioningDefinitions")]
public class SugarIdentityProvisioningDefinition
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Name { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Description { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string OptionsName { get; set; } = null!;
    public string OptionsFullQualifiedName { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarIdentityProvisioning.DefinitionName))]
    public List<SugarIdentityProvisioning> IdentityProvisionings { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarIdentityProvisioningMappingRule.IdentityProvisioningDefinitionName))]
    public List<SugarIdentityProvisioningMappingRule> MappingRules { get; set; }

    public static SugarIdentityProvisioningDefinition Transform(IdentityProvisioningDefinition definition)
    {
        return new SugarIdentityProvisioningDefinition
        {
            CreateDateTime = definition.CreateDateTime,
            Description = definition.Description,
            Name = definition.Name,
            OptionsFullQualifiedName = definition.OptionsFullQualifiedName,
            OptionsName = definition.OptionsName,
            UpdateDateTime = definition.UpdateDateTime,
            MappingRules = definition.MappingRules.Select(m => SugarIdentityProvisioningMappingRule.Transform(m)).ToList()
        };
    }

    public IdentityProvisioningDefinition ToDomain()
    {
        return new IdentityProvisioningDefinition
        {
            Name = Name,
            Description = Description,
            CreateDateTime = CreateDateTime,
            UpdateDateTime  = UpdateDateTime,
            OptionsName = OptionsName,
            OptionsFullQualifiedName  = OptionsFullQualifiedName,
            Instances = IdentityProvisionings == null ? new List<IdentityProvisioning>() : IdentityProvisionings.Select(r => r.ToDomain()).ToList(),
            MappingRules = MappingRules == null ? new List<IdentityProvisioningMappingRule>() : MappingRules.Select(r => r.ToDomain()).ToList()
        };
    }
}
