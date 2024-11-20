﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.ConformanceSuite.Startup.Converters
{
    public class FacebookOptionsLite : IDynamicAuthenticationOptions<FacebookOptions>
    {
        [SimpleIdServer.IdServer.ConfigurationRecord("AppId", "Application identifier", 0)]
        public string AppId { get; set; }
        [SimpleIdServer.IdServer.ConfigurationRecord("AppSecret", "Application secret", 1, null, SimpleIdServer.IdServer.CustomConfigurationRecordType.PASSWORD)]
        public string AppSecret { get; set; }

        public FacebookOptions Convert() => new FacebookOptions
        {
            AppId = AppId,
            AppSecret = AppSecret
        };
    }
}