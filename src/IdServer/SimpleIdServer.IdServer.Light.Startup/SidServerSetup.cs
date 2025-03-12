// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup;

public class SidServerSetup
{
    public static void ConfigureClientCredentials(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes);

        var app = webApplicationBuilder.Build();
        app.UseSID();
        app.Run();
    }

    public static void ConfigureAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            // .AddInMemoryRealms(Config.Realms)
            .AddInMemoryAuthenticationSchemes(Config.AuthenticationSchemes, Config.AuthenticationSchemeDefinitions)
            .AddPwdAuthentication(true);
            // .EnableRealm();

        var app = webApplicationBuilder.Build();
        app.UseSID();
        app.Run();
    }
}
