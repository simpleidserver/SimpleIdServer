// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup;

public class SidServerSetup
{
    public static void ConfigureClientCredentials(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes);

        var app = webApplicationBuilder.Build();
        app.UseSID();
        app.Run();
    }

    public static void ConfigureAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddPwdAuthentication(true);

        var app = webApplicationBuilder.Build();
        app.UseSID();
        app.Run();
    }
}
