// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class ConsentViewModel
    {
        public ConsentViewModel(string consentId, string clientName, string clientUri, IEnumerable<string> scopeNames, IEnumerable<string> claimNames)
        {
            ConsentId = consentId;
            ClientName = clientName;
            ClientUri = clientUri;
            ScopeNames = scopeNames;
            ClaimNames = claimNames;
        }

        public string ConsentId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public IEnumerable<string> ScopeNames { get; set; }
        public IEnumerable<string> ClaimNames { get; set; }
    }
}
