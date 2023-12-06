// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Startup.Converters
{
    public class FacebookOptionsLite : IDynamicAuthenticationOptions<FacebookOptions>
    {
        [SimpleIdServer.Configuration.ConfigurationRecord("AppId", "Application identifier", 0)]
        public string AppId { get; set; }
        [SimpleIdServer.Configuration.ConfigurationRecord("AppSecret", "Application secret", 1, null, Configuration.CustomConfigurationRecordType.PASSWORD)]
        public string AppSecret { get; set; }

        public FacebookOptions Convert() => new FacebookOptions
        {
            AppId = AppId,
            AppSecret = AppSecret
        };
    }
}