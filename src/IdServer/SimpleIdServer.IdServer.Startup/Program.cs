// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Startup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .AddInMemoryUsers(IdServerConfiguration.Users)
    .AddInMemoryScopes(IdServerConfiguration.Scopes)
    .AddInMemoryClients(IdServerConfiguration.Clients)
    .AddDeveloperSigningCredentials();

builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = "cookie";
        opts.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("cookie")
    .AddOpenIdConnect("oidc", opts =>
    {
        opts.Authority = "http://localhost:60001";
        opts.ClientId = "website";
        opts.ClientSecret = "password";
        opts.ResponseType = "code";
        opts.UsePkce = true;
        opts.ResponseMode = "query";
        opts.SaveTokens = true;
        opts.RequireHttpsMetadata = false;
    });

var app = builder.Build().UseSID();
app.Run();