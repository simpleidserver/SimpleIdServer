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
    [SugarColumn(IsNullable = true)]
    public string? Name { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? Description { get; set; }
    public string DefinitionName { get; set; } = null!;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    [Navigate(NavigateType.ManyToOne, nameof(DefinitionName))]
    public SugarIdentityProvisioningDefinition Definition { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarIdentityProvisioningHistory.IdentityProvisioningId))]
    public List<SugarIdentityProvisioningHistory> Histories { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarIdentityProvisioningProcess.IdentityProvisioningId))]
    public List<SugarIdentityProvisioningProcess> Processes { get; set; }
    [Navigate(typeof(SugarIdentityProvisioningRealm), nameof(SugarIdentityProvisioningRealm.IdentityProvisioningLstId), nameof(SugarIdentityProvisioningRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUser.IdentityProvisioningId))]
    public List<SugarUser> Users { get; set; }

    public static SugarIdentityProvisioning Transform(IdentityProvisioning identityProvisioning)
    {
        return new SugarIdentityProvisioning
        {
            Id = identityProvisioning.Id,
            Description = identityProvisioning.Description,
            Name = identityProvisioning.Name,
            IsEnabled = identityProvisioning.IsEnabled,
            CreateDateTime = identityProvisioning.CreateDateTime,
            UpdateDateTime = identityProvisioning.UpdateDateTime,
            DefinitionName = identityProvisioning.Definition.Name,
            Histories = identityProvisioning.Histories.Select(h => SugarIdentityProvisioningHistory.Transform(h)).ToList(),
            Processes = identityProvisioning.Processes.Select(p => SugarIdentityProvisioningProcess.Transform(p)).ToList(),
            Definition = SugarIdentityProvisioningDefinition.Transform(identityProvisioning.Definition),
            Realms = identityProvisioning.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList()
        };
    }

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
            Histories = Histories == null ? new List<IdentityProvisioningHistory>() : Histories.Select(h => h.ToDomain()).ToList(),
            Processes = Processes == null ? new List<IdentityProvisioningProcess>() : Processes.Select(p => p.ToDomain()).ToList(),
            Realms = Realms == null ? new List<Realm>() : Realms.Select(h => h.ToDomain()).ToList()
        };
    }
}
