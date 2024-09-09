// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Webfinger;
using SimpleIdServer.Webfinger.Apis;
using SimpleIdServer.Webfinger.Builders;
using SimpleIdServer.Webfinger.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddControllersWithViews();

builder.Services.AddWebfinger()
    .UseInMemoryEfStore(WebfingerResourceBuilder.New("acct", "jane@localhost:5020").AddLinkRelation("https://openid.net/specs/fastfed/1.0/provider", "https://localhost:5020").Build());

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute("getWebfinger",
                pattern: RouteNames.WellKnownWebFinger,
                defaults: new { controller = "Webfinger", action = "Get" });

app.Run();