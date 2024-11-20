﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("PresentationDefinitionInputDescriptor")]
public class SugarPresentationDefinitionInputDescriptor
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string PublicId { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Name { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Purpose { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? PresentationDefinitionId { get; set; } = null;

    [Navigate(NavigateType.OneToMany, nameof(SugarPresentationDefinitionFormat.PresentationDefinitionInputDescriptorId))]
    public List<SugarPresentationDefinitionFormat> Format { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarPresentationDefinitionInputDescriptorConstraint.PresentationDefinitionInputDescriptorId))]
    public List<SugarPresentationDefinitionInputDescriptorConstraint> Constraints { get; set; }

    public static SugarPresentationDefinitionInputDescriptor Transform(PresentationDefinitionInputDescriptor descriptor)
    {
        return new SugarPresentationDefinitionInputDescriptor
        {
            Id = descriptor.Id,
            PublicId = descriptor.PublicId,
            Name = descriptor.Name,
            Purpose = descriptor.Purpose,
            Format = descriptor.Format == null ? new List<SugarPresentationDefinitionFormat>() : descriptor.Format.Select(f => SugarPresentationDefinitionFormat.Transform(f)).ToList(),
            Constraints = descriptor.Constraints == null ? new List<SugarPresentationDefinitionInputDescriptorConstraint>() : descriptor.Constraints.Select(c => SugarPresentationDefinitionInputDescriptorConstraint.Transform(c)).ToList()
        };
    }

    public PresentationDefinitionInputDescriptor ToDomain()
    {
        return new PresentationDefinitionInputDescriptor
        {
            Id  = Id,
            PublicId = PublicId,
            Name = Name,
            Purpose = Purpose,
            Format = Format == null ? new List<PresentationDefinitionFormat>() : Format.Select(f => f.ToDomain()).ToList(),
            Constraints = Constraints == null ? new List<PresentationDefinitionInputDescriptorConstraint>() : Constraints.Select(f => f.ToDomain()).ToList()
        };
    }
}
