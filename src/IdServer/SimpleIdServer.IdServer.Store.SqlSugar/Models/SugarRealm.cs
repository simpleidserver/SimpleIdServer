// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Realms")]
public class SugarRealm
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "Name")]
    public string RealmsName { get; set; }
    public string Description { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    [Navigate(typeof(SugarApiResourceRealm), nameof(SugarApiResourceRealm.RealmsName), nameof(SugarApiResourceRealm.ApiResourcesId))]
    public List<SugarApiResource> ApiResources { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarPresentationDefinition.RealmName))]
    public List<SugarPresentationDefinition> PresentationDefinitions { get; set; }

    public static SugarRealm Transform(Realm realm) => new()
    {
        CreateDateTime = realm.CreateDateTime,
        UpdateDateTime = realm.UpdateDateTime,
        Description = realm.Description ?? realm.Name,
        RealmsName = realm.Name
    };

    public Realm ToDomain()
    {
        return new Realm
        {
            Name = RealmsName,
            Description = Description,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime
        };
    }
}