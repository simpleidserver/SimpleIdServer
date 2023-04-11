// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using SimpleIdServer.IdServer.Serializer;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Startup.Converters
{
    public class FacebookOptionsLite : IDynamicAuthenticationOptions<FacebookOptions>
    {
        [SimpleIdServer.IdServer.Serializer.VisibleProperty("AppId")]
        public string AppId { get; set; }
        [SimpleIdServer.IdServer.Serializer.VisibleProperty("AppSecret")]
        public string AppSecret { get; set; }

        public FacebookOptions Convert() => new FacebookOptions
        {
            AppId = AppId,
            AppSecret = AppSecret
        };
    }
}