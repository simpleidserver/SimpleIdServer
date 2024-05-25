// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("IdentityProvisioningLst")]
public class SugarIdentityProvisioning
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string DefinitionName { get; set; } = null!;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    [Navigate(NavigateType.ManyToOne, nameof(DefinitionName))]
    public SugarIdentityProvisioningDefinition Definition { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarIdentityProvisioningHistory.IdentityProvisioningId))]
    public List<SugarIdentityProvisioningHistory> Histories { get; set; }
    [Navigate(typeof(SugarIdentityProvisioningRealm), nameof(SugarIdentityProvisioningRealm.IdentityProvisioningLstId), nameof(SugarIdentityProvisioningRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUser.IdentityProvisioningId))]
    public List<SugarUser> Users { get; set; }

    public IdentityProvisioning ToDomain()
    {
        return new IdentityProvisioning
        {
            Id = Id,
            Name = Name,
            Description = Description,
            IsEnabled = IsEnabled,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            Definition = Definition?.ToDomain(),
            Histories = Histories.Select(h => h.ToDomain()).ToList(),
            Realms = Realms.Select(h => h.ToDomain()).ToList()
        };
    }
}
