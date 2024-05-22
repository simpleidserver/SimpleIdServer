// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("PresentationDefinitionInputDescriptorConstraint")]
public class SugarPresentationDefinitionInputDescriptorConstraint
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    public string Path { get; set; } = null!;
    public string Filter { get; set; } = null!;
    public string PresentationDefinitionInputDescriptorId { get; set; }

    public PresentationDefinitionInputDescriptorConstraint ToDomain()
    {
        return new PresentationDefinitionInputDescriptorConstraint
        {
            Id = Id,
            Path = Path.Split(',').ToList(),
            Filter = Filter
        };
    }
}
