// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.OpenID.UI.AuthProviders
{
    public class SIDAuthenticationScheme
    {
        public SIDAuthenticationScheme(AuthenticationScheme authScheme)
        {
            AuthScheme = authScheme;
        }

        public SIDAuthenticationScheme(AuthenticationScheme authScheme, object optionsMonitor) : this(authScheme)
        {
            OptionsMonitor = optionsMonitor;
        }


        public AuthenticationScheme AuthScheme { get; set; }
        public object OptionsMonitor { get; set; }
    }
}
