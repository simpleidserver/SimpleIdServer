// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class IdentityProvisioning
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public IdentityProvisioningDefinition Definition { get; set; } = null!;
        public ICollection<IdentityProvisioningProperty> Properties { get; set; } = new List<IdentityProvisioningProperty>();
        public ICollection<IdentityProvisioningHistory> Histories { get; set; } = new List<IdentityProvisioningHistory>();
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();

        public void Export(DateTime startDateTime, DateTime endDateTime, string folderName, int nbRepresentations)
        {
            Histories.Add(new IdentityProvisioningHistory
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                FolderName = folderName,
                NbRepresentations = nbRepresentations,
                Status = IdentityProvisioningHistoryStatus.EXPORT
            });
        }

        public void FailToImport(DateTime startDateTime, DateTime endDateTime, string errorMessage)
        {
            Histories.Add(new IdentityProvisioningHistory
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ErrorMessage = errorMessage,
                Status = IdentityProvisioningHistoryStatus.ERROR
            });
        }
    }
}
