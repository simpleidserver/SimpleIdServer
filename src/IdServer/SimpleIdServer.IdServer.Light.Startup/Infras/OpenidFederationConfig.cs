// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public static class OpenidFederationConfig
{
    public static void ConfigureOpenidFederation(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddOpenidFederation(null);
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }
}
