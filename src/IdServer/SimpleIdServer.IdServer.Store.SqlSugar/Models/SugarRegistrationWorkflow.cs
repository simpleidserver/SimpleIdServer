// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("RegistrationWorkflows")]
public class SugarRegistrationWorkflow
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string RealmName { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string? WorkflowId { get; set; } = null;
    public bool IsDefault { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarApiResourceRealm.RealmsName), nameof(SugarApiResourceRealm.ApiResourcesId))]
    public SugarRealm Realm { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthenticationContextClassReference.RegistrationWorkflowId))]
    public List<SugarAuthenticationContextClassReference> Acrs { get; set; }

    public static SugarRegistrationWorkflow Transform(RegistrationWorkflow record)
    {
        return new SugarRegistrationWorkflow
        {
            Id = record.Id,
            CreateDateTime = record.CreateDateTime,
            Name = record.Name,
            UpdateDateTime = record.UpdateDateTime,
            WorkflowId = record.WorkflowId,
            IsDefault = record.IsDefault,
            RealmName = record.RealmName ?? record.Realm?.Name,
            Acrs = record.Acrs == null ? new List<SugarAuthenticationContextClassReference>() : record.Acrs.Select(a => new SugarAuthenticationContextClassReference
            {
                Id = a.Id,
                Name = a.Name,
                DisplayName = a.DisplayName,
            }).ToList()
        };
    }

    public RegistrationWorkflow ToDomain()
    {
        return new RegistrationWorkflow
        {
            Id = Id,
            Name = Name,
            RealmName = RealmName,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            WorkflowId = WorkflowId,
            IsDefault = IsDefault,
            Acrs = Acrs == null ? new List<AuthenticationContextClassReference>() : Acrs.Select(a => a.ToDomain()).ToList(),
            Realm = Realm?.ToDomain()
        };
    }
}
