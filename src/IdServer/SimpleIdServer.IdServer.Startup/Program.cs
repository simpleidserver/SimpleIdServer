// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Startup;

// TODO
// Filter authentication scheme.
// Can choose a session.
// Review the authentication scheme...

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .AddInMemoryUsers(IdServerConfiguration.Users)
    .AddInMemoryScopes(IdServerConfiguration.Scopes)
    .AddInMemoryClients(IdServerConfiguration.Clients)
    .AddDeveloperSigningCredentials()
    .AddAuthentication(callback: (a) =>
    {
        a.AddSelfAuthentication(opts =>
        {
            opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.Authority = "http://localhost:60001";
            opts.ClientId = "website";
            opts.ClientSecret = "password";
            opts.ResponseType = "code";
            opts.UsePkce = true;
            opts.ResponseMode = "query";
            opts.SaveTokens = true;
            opts.RequireHttpsMetadata = false;
        });
    });

var app = builder.Build();
app.UseSID();
app.Run();