// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("IdentityProvisioningHistory")]
public class SugarIdentityProvisioningHistory
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string ProcessId { get; set; } = null!;
    public DateTime ExecutionDateTime { get; set; }
    public int CurrentPage { get; set; }
    public int NbUsers { get; set; }
    public int NbGroups { get; set; }
    public int NbFilteredRepresentations { get; set; }
    public int TotalPages { get; set; }
    public IdentityProvisioningHistoryStatus Status { get; set; }
    public string IdentityProvisioningId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(IdentityProvisioningId))]
    public IdentityProvisioning IdentityProvisioning { get; set; } = null!;

    public IdentityProvisioningHistory ToDomain()
    {
        return new IdentityProvisioningHistory
        {
            ProcessId = ProcessId,
            ExecutionDateTime = ExecutionDateTime,
            CurrentPage = CurrentPage,
            NbUsers = NbUsers,
            NbGroups = NbGroups,
            NbFilteredRepresentations = NbFilteredRepresentations,
            TotalPages = TotalPages,
            Status = Status
        };
    }
}
