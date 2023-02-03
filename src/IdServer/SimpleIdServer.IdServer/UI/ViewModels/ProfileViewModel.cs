// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public bool HasOtpKey { get; set; }
        public IEnumerable<ConsentViewModel> Consents { get; set; }
        public IEnumerable<PendingRequestViewModel> PendingRequests { get; set; }
        public IEnumerable<ExternalAuthProviderViewModel> Profiles { get; set; }
        public IEnumerable<ExternalIdProvider> ExternalIdProviders { get; set; }
    }
}
