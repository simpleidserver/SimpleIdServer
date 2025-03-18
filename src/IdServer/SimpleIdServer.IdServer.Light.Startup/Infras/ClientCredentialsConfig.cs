// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Light.Startup.Migrations.AfterDeployment;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public static class ClientCredentialsConfig
{
    public static void Run(WebApplicationBuilder webApplicationBuilder)
    {
        var scope = ScopeBuilder.CreateApiScope("api1", false).Build();
        var clients = new List<Client>
        {
            ClientBuilder.BuildApiClient("client", "secret").AddScope(scope).Build()
        };
        var scopes = new List<Scope>
        {
            scope
        };
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(clients)
            .AddInMemoryScopes(scopes);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void RunWithEf(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddTransient<IDataSeeder, InitDataSeeder>();
        webApplicationBuilder.AddSidIdentityServer()
            .UseEfStore(db => db.UseSqlite("Data Source=Application.db;"), db => db.UseSqlite("Data Source=Application.db;"));

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Services.SeedData();
        app.Run();
    }
}