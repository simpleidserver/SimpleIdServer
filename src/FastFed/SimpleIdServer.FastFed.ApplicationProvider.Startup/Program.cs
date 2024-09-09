// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddAntiforgery();
builder.Services.AddFastFedApplicationProvider();
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllerRoute(
name: "defaultWithArea",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();