// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Sms;

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
        app.UseSid();
        app.Run();
    }

    public static void ConfigurePwdAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddInMemoryAuthenticationSchemes(Config.AuthenticationSchemes, Config.AuthenticationSchemeDefinitions)
            .AddPwdAuthentication(true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigurePwdAuthenticationWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddInMemoryRealms(Config.Realms)
            .AddInMemoryAuthenticationSchemes(Config.AuthenticationSchemes, Config.AuthenticationSchemeDefinitions)
            .AddPwdAuthentication(true, true)
            .EnableRealm();

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureEmailAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddEmailAuthentication(true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureEmailAuthenticationWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddInMemoryRealms(Config.Realms)
            .AddEmailAuthentication(true, true)
            .EnableRealm();

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureOtpAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddOtpAuthentication(true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureOtpAuthenticationWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryRealms(Config.Realms)
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddOtpAuthentication(true, true)
            .EnableRealm();

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureSmsAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddSmsAuthentication(true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureSmsAuthenticationWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryRealms(Config.Realms)
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddSmsAuthentication(true, true)
            .EnableRealm();

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureMobileAuthentication(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryRealms(Config.Realms)
            .AddInMemoryUsers(Config.Users)
            .AddInMemoryLanguages(Config.Languages)
            .AddMobileAuthentication(null, true, true);

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureWebauthnAuthentication(WebApplicationBuilder webApplicationBuilder)
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

    public static void ConfigureWsFederation(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddInMemoryUsers(Config.Users)
            .AddPwdAuthentication(true, true)
            .AddWsFederation();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureSaml(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddInMemoryUsers(Config.Users)
            .AddPwdAuthentication(true, true)
            .AddSamlIdp();

        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureSwagger(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer(o =>
            {
                o.Authority = "https://localhost:5001";
            })
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddSwagger();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.UseSidSwagger();
        app.UseSidSwaggerUi();
        app.Run();
    }

    public static void ConfigureSwaggerWithRealm(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer(o =>
        {
            o.Authority = "https://localhost:5001";
        })
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddInMemoryRealms(Config.Realms)
            .EnableRealm()
            .AddSwagger();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.UseSidSwagger();
        app.UseSidSwaggerUi();
        app.Run();
    }

    public static void ConfigureLdapProvisioning(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddLdapProvisioning();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureScimProvisioning(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddScimProvisioning();
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }

    public static void ConfigureOpenidFederation(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes)
            .AddOpenidFederation(null, true);
        var app = webApplicationBuilder.Build();
        app.UseSid();
        app.Run();
    }
}