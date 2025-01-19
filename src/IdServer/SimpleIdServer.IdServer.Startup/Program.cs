// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Startup.Conf;
using System.Net;

ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
var identityServerConfiguration = builder.Configuration.Get<IdentityServerConfiguration>();
builder.Services.ConfigureKestrel(identityServerConfiguration);
builder.Services.ConfigureForwardedHeaders(identityServerConfiguration);
builder.Services.ConfigureClientCertificateForwarding(identityServerConfiguration);
builder.Services.ConfigureCors();
builder.Services.ConfigureRazorAndLocalization();
SidServerSetup.ConfigureIdServer(builder, identityServerConfiguration);
SidServerSetup.ConfigureCentralizedConfiguration(builder);

// Uncomment these two lines to enable seed data from JSON file.
// builder.Services.AddJsonSeeding(builder.Configuration);
// builder.Services.AddEntitySeeders(typeof(UserEntitySeeder));
var app = builder.Build();
DataSeeder.SeedData(app, builder.Configuration["SCIM:SCIMRepresentationsExtractionJobOptions:SCIMEdp"]);
app.UseCors("AllowAll");
if (identityServerConfiguration.IsForwardedEnabled) app.UseForwardedHeaders();
if (identityServerConfiguration.ForceHttps) app.SetHttpsScheme();
app.UseRequestLocalization(e =>
{
    e.SetDefaultCulture("en");
    e.AddSupportedCultures("en", "fr");
    e.AddSupportedUICultures("en", "fr");
});

if (!app.Environment.IsDevelopment())
{
    var errorPath = identityServerConfiguration.IsRealmEnabled ? "/master/Error/Unexpected" : "/Error/Unexpected";
    app.UseExceptionHandler(errorPath);
}

if(identityServerConfiguration.IsClientCertificateForwarded) app.UseCertificateForwarding();
app.MapBlazorHub();
app
    .UseSID()
    .UseVerifiablePresentation()
    .UseSIDSwagger()
    .UseSIDSwaggerUI()
    // .UseSIDReDoc()
    .UseWsFederation()
    .UseFIDO()
    .UseSamlIdp()
    .UseGotifyNotification()
    .UseAutomaticConfiguration()
    .UseOpenidFederation();

app.Run();