// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Startup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .AddInMemoryUsers(IdServerConfiguration.Users)
    .AddInMemoryScopes(IdServerConfiguration.Scopes)
    .AddInMemoryClients(IdServerConfiguration.Clients)
    .AddDeveloperSigningCredentials()
    .AddAuthentication(callback: (a) =>
    {
        a.AddOIDCAuthentication(opts =>
        {
            opts.Authority = "http://localhost:60001";
            opts.ClientId = "website";
            opts.ClientSecret = "password";
            opts.ResponseType = "code";
            opts.UsePkce = true;
            opts.ResponseMode = "query";
            opts.SaveTokens = true;
            opts.GetClaimsFromUserInfoEndpoint = true;
            opts.RequireHttpsMetadata = false;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name"
            };
            opts.Scope.Add("profile");
        });
        a.Builder.AddFacebook(o =>
        {
            o.AppId = "569242033233529";
            o.AppSecret = "12e0f33817634c0a650c0121d05e53eb";
        });
    });

var app = builder.Build();
app.UseSID();
app.Run();