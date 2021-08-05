// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.OpenID.Startup.Converters
{
    public class FacebookPostConfigureOptions : OAuthPostConfigureOptions<FacebookOptions, FacebookHandler>
    {
        public FacebookPostConfigureOptions(IDataProtectionProvider dataProtection) : base(dataProtection)
        {
        }
    }
}
