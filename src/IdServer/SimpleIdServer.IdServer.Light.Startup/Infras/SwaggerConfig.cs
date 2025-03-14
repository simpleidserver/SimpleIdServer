// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Light.Startup.Infras;

public static class SwaggerConfig
{
    public static void Run(WebApplicationBuilder webApplicationBuilder)
    {
        var clients = new List<Client>
        {
            SwaggerClientBuilder.Build("swagger", "password", new List<Scope>(), "https://localhost:5001/swagger/oauth2-redirect.html")
        };
        var scopes = new List<Scope>
        {
            Constants.DefaultScopes.Provisioning,
            Constants.DefaultScopes.Users,
            Constants.DefaultScopes.Acrs,
            Constants.DefaultScopes.ConfigurationsScope,
            Constants.DefaultScopes.AuthenticationSchemeProviders,
            Constants.DefaultScopes.AuthenticationMethods,
            Constants.DefaultScopes.RegistrationWorkflows,
            Constants.DefaultScopes.ApiResources,
            Constants.DefaultScopes.Auditing,
            Constants.DefaultScopes.Scopes,
            Constants.DefaultScopes.CertificateAuthorities,
            Constants.DefaultScopes.Clients,
            Constants.DefaultScopes.Realms,
            Constants.DefaultScopes.Groups
        };
        webApplicationBuilder.AddSidIdentityServer(o =>
        {
            o.Authority = "https://localhost:5001";
        })
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(clients)
            .AddInMemoryScopes(scopes)
            .AddInMemoryUsers(Config.Users)
            .AddPwdAuthentication(true, true)
            .AddSwagger();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.UseSidSwagger();
        app.UseSidSwaggerUi();
        app.Run();
    }

    public static void RunWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        var clients = new List<Client>
        {
            SwaggerClientBuilder.Build("swagger", "password", new List<Scope>(), "https://localhost:5001/(.*)/swagger/oauth2-redirect.html")
        };
        var scopes = new List<Scope>
        {
            Constants.DefaultScopes.Provisioning,
            Constants.DefaultScopes.Users,
            Constants.DefaultScopes.Acrs,
            Constants.DefaultScopes.ConfigurationsScope,
            Constants.DefaultScopes.AuthenticationSchemeProviders,
            Constants.DefaultScopes.AuthenticationMethods,
            Constants.DefaultScopes.RegistrationWorkflows,
            Constants.DefaultScopes.ApiResources,
            Constants.DefaultScopes.Auditing,
            Constants.DefaultScopes.Scopes,
            Constants.DefaultScopes.CertificateAuthorities,
            Constants.DefaultScopes.Clients,
            Constants.DefaultScopes.Realms,
            Constants.DefaultScopes.Groups
        };
        webApplicationBuilder.AddSidIdentityServer(o =>
        {
            o.Authority = "https://localhost:5001";
        })
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(clients)
            .AddInMemoryScopes(scopes)
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryRealms(Config.Realms)
            .AddPwdAuthentication(true, true)
            .EnableRealm()
            .AddSwagger();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.UseSidSwagger();
        app.UseSidSwaggerUi();
        app.Run();
    }
}
