// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Startup.Converters
{
    public class OpenIdConnectLiteOptions : IDynamicAuthenticationOptions<OpenIdConnectOptions>
    {
        [SimpleIdServer.Configuration.ConfigurationRecord("ClientId", "Client Identifier", order: 0)]
        public string ClientId { get; set; }
        [SimpleIdServer.Configuration.ConfigurationRecord("ClientSecret", "Client Secret", order: 1)]
        public string ClientSecret { get; set; }

        public OpenIdConnectOptions Convert()
        {
            return null;
        }
    }
}
