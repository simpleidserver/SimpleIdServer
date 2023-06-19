// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class CredentialsViewModel
    {
        public ICollection<CredentialTemplateViewModel> Credentials { get; set; } = new List<CredentialTemplateViewModel>();
        public IEnumerable<string> ClientIds { get; set; }
    }
}
