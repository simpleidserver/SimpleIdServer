// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ApiResources")]
public class SugarApiResource
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Audience { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    [Navigate(typeof(SugarApiResourceScope), nameof(SugarApiResourceScope.ApiResourcesId), nameof(SugarApiResourceScope.ScopesId))]
    public List<SugarScope> Scopes { get; set; } = new List<SugarScope>();
    [Navigate(typeof(SugarApiResourceRealm), nameof(SugarApiResourceRealm.ApiResourcesId), nameof(SugarApiResourceRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }

    public ApiResource ToDomain()
    {
        return new ApiResource
        {
            Id = Id,
            Name = Name,
            Audience = Audience,
            Description = Description,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            Scopes = Scopes == null ? new List<Scope>() : Scopes.Select(s => s.ToDomain()).ToList(),
            Realms = Realms == null ? new List<Realm>() : Realms.Select(r => r.ToDomain()).ToList()
        };
    }
}