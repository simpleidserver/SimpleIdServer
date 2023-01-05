// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class ConsentViewModel
    {
        public ConsentViewModel(string consentId, string clientName, IEnumerable<string> scopeNames, IEnumerable<string> claimNames)
        {
            ConsentId = consentId;
            ClientName = clientName;
            ScopeNames = scopeNames;
            ClaimNames = claimNames;
        }

        public string ConsentId { get; set; }
        public string ClientName { get; set; }
        public IEnumerable<string> ScopeNames { get; set; }
        public IEnumerable<string> ClaimNames { get; set; }
    }
}
