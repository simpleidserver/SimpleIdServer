// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdServerLogging;
using IdServerLogging.Conf;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var webApplicationBuilder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .CreateLogger();
webApplicationBuilder.Logging.AddSerilog();
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