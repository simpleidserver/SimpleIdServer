// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class ConsentsIndexViewModel
    {
        public ConsentsIndexViewModel(string clientName, string returnUrl, string pictureUri, IEnumerable<string> scopeNames, IEnumerable<string> claimNames)
        {
            ClientName = clientName;
            ReturnUrl = returnUrl;
            PictureUri = pictureUri;
            ScopeNames = scopeNames;
            ClaimNames = claimNames;
        }

        public string ClientName { get; set; }
        public string ReturnUrl { get; set; }
        public string PictureUri { get; set; }
        public IEnumerable<string> ScopeNames { get; set; }
        public IEnumerable<string> ClaimNames { get; set; }
    }
}
