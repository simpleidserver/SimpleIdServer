// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.UI.ViewModels
{
    public class ConsentsIndexViewModel
    {
        public ConsentsIndexViewModel(string clientName, string returnUrl, IEnumerable<string> scopeDescriptions, IEnumerable<string> claimNames, string cancellationUrl)
        {
            ClientName = clientName;
            ReturnUrl = returnUrl;
            ScopeDescriptions = scopeDescriptions;
            ClaimNames = claimNames;
            CancellationUrl = cancellationUrl;
        }

        public string ClientName { get; set; }
        public string ReturnUrl { get; set; }
        public string CancellationUrl { get; set; }
        public IEnumerable<string> ScopeDescriptions { get; set; }
        public IEnumerable<string> ClaimNames { get; set; }
    }
}
