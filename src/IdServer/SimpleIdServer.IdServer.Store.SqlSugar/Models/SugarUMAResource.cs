// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UmaResources")]
public class SugarUMAResource
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string? IconUri { get; set; } = null;
    public string? Type { get; set; } = null;
    public string? Subject { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string Realm { get; set; } = null!;
    public string Scopes { get; set; } = null!;
    [Navigate(typeof(SugarTranslationUMAResource), nameof(SugarTranslationUMAResource.UMAResourceId), nameof(SugarTranslationUMAResource.TranslationsId))]
    public List<SugarTranslation> Translations { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUMAResourcePermission.UMAResourceId))]
    public List<SugarUMAResourcePermission> Permissions { get; set; }

    public UMAResource ToDomain()
    {
        return new UMAResource
        {
            CreateDateTime = CreateDateTime,
            IconUri = IconUri,
            Type = Type,
            Subject = Subject,
            Id = Id,
            Realm = Realm,
            Scopes = Scopes.Split(',').ToList(),
            UpdateDateTime = UpdateDateTime,
            Translations = Translations.Select(t => t.ToDomain()).ToList(),
            Permissions = Permissions.Select(p => p.ToDomain()).ToList()
        };
    }
}
