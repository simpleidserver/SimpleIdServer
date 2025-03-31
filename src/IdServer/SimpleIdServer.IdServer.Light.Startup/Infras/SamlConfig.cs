// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public static class SamlConfig
{
    public static void Run(WebApplicationBuilder webApplicationBuilder)
    {
        var clients = new List<Client>
        {
            SamlSpClientBuilder.BuildSamlSpClient("samlSp", "http://localhost:5125/Metadata").Build()
        };
        var scopes = new List<Scope>
        {
            IdServer.Config.DefaultScopes.SAMLProfile
        };
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(clients)
            .AddInMemoryScopes(scopes)
            .AddInMemoryUsers(Config.Users)
            .AddPwdAuthentication(true)
            .AddSamlIdp();

        var app = webApplicationBuilder.Build();
        app.Services.SeedData();
        app.UseSid();
        app.Run();
    }
}
