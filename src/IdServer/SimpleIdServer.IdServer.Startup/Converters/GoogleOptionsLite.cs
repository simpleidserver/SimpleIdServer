// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Google;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Startup.Converters;

public class GoogleOptionsLite : IDynamicAuthenticationOptions<GoogleOptions>
{
    [SimpleIdServer.IdServer.ConfigurationRecord("ClientId", "Client identifier", 0, IsRequired = true)]
    public string ClientId { get; set; }
    [SimpleIdServer.IdServer.ConfigurationRecord("ClientSecret", "Client secret", 1, null, SimpleIdServer.IdServer.CustomConfigurationRecordType.PASSWORD, IsRequired = true)]
    public string ClientSecret { get; set; }

    public GoogleOptions Convert() => new GoogleOptions
    {
        ClientId = ClientId,
        ClientSecret = ClientSecret
    };
}
