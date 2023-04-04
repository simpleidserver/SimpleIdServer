// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class IdentityProvisioningHistory
    {
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? FolderName { get; set; } = null;
        public int NbRepresentations { get; set; }
        public IdentityProvisioningHistoryStatus Status { get; set; }
        public string? ErrorMessage { get; set; } = null;
        public IdentityProvisioning IdentityProvisioning { get; set; } = null!;

        public void Import()
        {
            Status = IdentityProvisioningHistoryStatus.IMPORT;
        }
    }

    public enum IdentityProvisioningHistoryStatus
    {
        START = 0,
        EXPORT = 1,
        ERROR = 2,
        IMPORT = 3
    }
}
