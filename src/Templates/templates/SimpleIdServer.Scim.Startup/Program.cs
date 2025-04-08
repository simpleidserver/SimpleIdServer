// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Startup.Configurations;
using SimpleIdServer.Scim.Startup.Infrastructures;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
var conf = builder.Configuration.Get<StorageConfiguration>();
var apiKeysConf = builder.Configuration.Get<ApiKeysConfiguration>();
ScimServerSetup.ConfigureScim(builder, conf, apiKeysConf);
var app = builder.Build();
Dataseeder.Seed(builder, app, conf);
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
});
app.UseScim();