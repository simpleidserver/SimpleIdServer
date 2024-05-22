// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("RegistrationWorkflows")]
public class SugarRegistrationWorkflow
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string RealmName { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string Steps { get; set; } = null!;
    public bool IsDefault { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarApiResourceRealm.RealmsName), nameof(SugarApiResourceRealm.ApiResourcesId))]
    public SugarRealm Realm { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthenticationContextClassReference.RegistrationWorkflowId)]
    public List<SugarAuthenticationContextClassReference> Acrs { get; set; }

    public RegistrationWorkflow ToDomain()
    {
        return new RegistrationWorkflow
        {
            Id = Id,
            Name = Name,
            RealmName = RealmName,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            Steps = Steps.Split(',').ToList(),
            IsDefault = IsDefault,

        };
    }
}
