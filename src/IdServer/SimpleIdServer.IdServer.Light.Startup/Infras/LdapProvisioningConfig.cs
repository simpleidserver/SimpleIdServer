// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public class LdapProvisioningConfig
{
    public static void Run(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddLdapProvisioning();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }
}
