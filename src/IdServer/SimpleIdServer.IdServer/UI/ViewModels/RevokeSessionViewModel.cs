// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class RevokeSessionViewModel
    {
        public RevokeSessionViewModel(string revokeSessionCallbackUrl, JsonWebToken idTokenHint, string frontChannelLogout)
        {
            RevokeSessionCallbackUrl = revokeSessionCallbackUrl;
            IdTokenHint = idTokenHint;
            FrontChannelLogout = frontChannelLogout;
        }

        public string RevokeSessionCallbackUrl { get; set; }
        public JsonWebToken IdTokenHint { get; set; }
        public string FrontChannelLogout { get; set; }
    }
}
