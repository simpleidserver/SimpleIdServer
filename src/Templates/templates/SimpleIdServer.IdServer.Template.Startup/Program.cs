// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleIdServer.IdServer.Template.Startup;
using SimpleIdServer.IdServer.Template.Startup.Conf;

var webApplicationBuilder = WebApplication.CreateBuilder(args);
webApplicationBuilder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
webApplicationBuilder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
var identityServerConfiguration = webApplicationBuilder.Configuration.Get<IdentityServerConfiguration>();
SidServerSetup.ConfigureIdServer(webApplicationBuilder, identityServerConfiguration);

var app = webApplicationBuilder.Build();
app.Services.SeedData();
var hostedServices = app.Services.GetServices<IHostedService>();
app.UseCors("AllowAll");
app.UseSid()
    .UseSidSwagger()
    .UseSidSwaggerUi();
app.Run();