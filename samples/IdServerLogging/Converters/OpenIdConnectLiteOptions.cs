// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace IdServerLogging.Converters
{
    public class OpenIdConnectLiteOptions : IDynamicAuthenticationOptions<OpenIdConnectOptions>
    {
        [ConfigurationRecord("ClientId", "Client Identifier", order: 0, IsRequired = true)]
        public string ClientId { get; set; }
        [ConfigurationRecord("ClientSecret", "Client Secret", order: 1, IsRequired = true)]
        public string ClientSecret { get; set; }

        public OpenIdConnectOptions Convert()
        {
            return null;
        }
    }
}
