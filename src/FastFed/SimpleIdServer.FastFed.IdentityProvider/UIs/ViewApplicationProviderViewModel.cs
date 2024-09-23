using SimpleIdServer.FastFed.Models;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs
{
    public class ViewApplicationProviderViewModel
    {
        public string EntityId { get; set; }
        public IdentityProviderStatus Status { get; set; }
        public List<ProvisioningProfileViewModel> ProvisioningProfiles { get; set; } = new List<ProvisioningProfileViewModel>();
        public List<ImportErrorViewModel> ImportErrors { get; set; } = new List<ImportErrorViewModel>();
    }

    public class ProvisioningProfileViewModel
    {
        public string ProfileName { get; set; }
        public int NbRecords { get; set; }
    }

    public class ImportErrorViewModel
    {
        public string ErrorMessage { get; set; }
        public string ExtractedRepresentationId { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
