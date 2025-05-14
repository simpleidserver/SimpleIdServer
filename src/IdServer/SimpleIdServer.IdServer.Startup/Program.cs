// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Startup.Conf;


var hashedPwd = "AQAAAAIAAYagAAAAEJwcFDmZdWMYEMLLTWiEfYvLsAUFtsMPR3/JMKIE9Zh4o76Wl2020g4stjHqv05zwg==";
var pp = "Pass123$";
var hasher = new PasswordHasher<User>();
var b = hasher.VerifyHashedPassword(new User(), hashedPwd, pp);
if(b == PasswordVerificationResult.Success)
{

}

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