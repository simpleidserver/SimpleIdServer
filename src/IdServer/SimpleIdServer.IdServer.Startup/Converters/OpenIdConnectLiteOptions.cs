// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SimpleIdServer.IdServer.Serializer;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Startup.Converters
{
    public class OpenIdConnectLiteOptions : IDynamicAuthenticationOptions<OpenIdConnectOptions>
    {
        [VisibleProperty("ClientId")]
        public string ClientId { get; set; }
        [VisibleProperty("ClientSecret")]
        public string ClientSecret { get; set; }

        public OpenIdConnectOptions Convert()
        {
            return null;
        }
    }
}
