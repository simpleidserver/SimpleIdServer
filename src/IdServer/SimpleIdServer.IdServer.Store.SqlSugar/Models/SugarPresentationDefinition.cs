// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("PresentationDefinitions")]
public class SugarPresentationDefinition
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string PublicId { get; set; } = null!;
    public string? Name { get; set; } = null;
    public string? Purpose { get; set; } = null;
    public string RealmName { get; set; } = null!;
    public SugarRealm Realm { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarPresentationDefinitionInputDescriptor.PresentationDefinitionId))]
    public List<SugarPresentationDefinitionInputDescriptor> InputDescriptors { get; set; }

    public PresentationDefinition ToDomain()
    {
        return new PresentationDefinition
        {
            Id= Id,
            PublicId= PublicId,
            Name= Name,
            RealmName= RealmName,
            Purpose= Purpose,
            InputDescriptors = InputDescriptors == null ? new List<PresentationDefinitionInputDescriptor>() : InputDescriptors.Select(i => i.ToDomain()).ToList()
        };
    }
}
