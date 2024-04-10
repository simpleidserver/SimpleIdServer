// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class RevokeSessionViewModel
    {
        public RevokeSessionViewModel(
            string revokeSessionCallbackUrl, 
            JsonWebToken idTokenHint, 
            IEnumerable<string> frontChannelLogouts,
            bool redirectToRevokeSessionUI)
        {
            RevokeSessionCallbackUrl = revokeSessionCallbackUrl;
            IdTokenHint = idTokenHint;
            FrontChannelLogouts = frontChannelLogouts;
            RedirectToRevokeSessionUI = redirectToRevokeSessionUI;
        }

        public string RevokeSessionCallbackUrl { get; set; }
        public JsonWebToken IdTokenHint { get; set; }
        public IEnumerable<string> FrontChannelLogouts { get; set; }
        public bool RedirectToRevokeSessionUI { get; set; }
    }
}
