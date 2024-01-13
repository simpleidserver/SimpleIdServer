// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class IdentityProvisioningHistory
{
    public string ProcessId { get; set; } = null!;
    public DateTime ExecutionDateTime { get; set; }
    public int CurrentPage { get; set; }
    public int NbUsers { get; set; }
    public int NbGroups { get; set; }
    public int NbFilteredRepresentations { get; set; }
    public int TotalPages { get; set; }
    public IdentityProvisioningHistoryStatus Status { get; set; }
    public IdentityProvisioning IdentityProvisioning { get; set; } = null!;
}

public enum IdentityProvisioningHistoryStatus
{
    CREATE = 0,
    START = 1,
    EXPORT = 2,
    FINISHEXPORT = 3,
    STARTIMPORT = 4,
    IMPORT = 5,
    FINISHIMPORT = 6
}
