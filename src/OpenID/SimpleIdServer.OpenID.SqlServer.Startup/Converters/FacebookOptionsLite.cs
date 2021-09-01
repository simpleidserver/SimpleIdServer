using Microsoft.AspNetCore.Authentication.Facebook;
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID.SqlServer.Startup.Converters
{
    public class FacebookOptionsLite
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }

        public FacebookOptions ToOptions()
        {
            return new FacebookOptions
            {
                AppId = AppId,
                AppSecret = AppSecret,
                SignInScheme = SIDOpenIdConstants.ExternalAuthenticationScheme
            };
        }

        public static FacebookOptionsLite Create(FacebookOptions opts)
        {
            return new FacebookOptionsLite
            {
                AppId = opts.AppId,
                AppSecret = opts.AppSecret
            };
        }
    }
}
