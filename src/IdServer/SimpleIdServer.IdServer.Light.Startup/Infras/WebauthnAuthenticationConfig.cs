// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public static class WebauthnAuthenticationConfig
{
    public static void Run(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryRealms(Config.Realms)
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddWebauthnAuthentication(null, true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }
}
