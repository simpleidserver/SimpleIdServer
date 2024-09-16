// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;
using SimpleIdServer.FastFed.Store.EF;
using SimpleIdServer.Webfinger;
using SimpleIdServer.Webfinger.Builders;
using System.Collections.Generic;

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
    .UseInMemoryEfStore(WebfingerResourceBuilder.New("acct", "jane@localhost:5020").AddLinkRelation("https://openid.net/specs/fastfed/1.0/provider", "https://localhost:5020/fastfed").Build());
builder.Services.AddFastFed(o =>
{
    o.ProviderDomain = "localhost:5020";
    o.IdProvider = new SimpleIdServer.FastFed.IdProviderOptions
    {
        Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
        {
            ProvisioningProfiles = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
        },
            SchemaGrammars = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:schemas:scim:2.0"
        },
            SigningAlgorithms = new List<string>
        {
            "RS256"
        }
        },
        ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
        {
            Email = "support@example.com",
            Organization = "Example Inc.",
            Phone = "+1-800-555-5555"
        },
        DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
        {
            DisplayName = "Example Identity Provider",
            LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
            License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
        }
    };
})
    .AddFastFedIdentityProvider(cbChooser: (t) => t.UseInMemoryEfStore()) // configure EF
    .AddIdProviderScimProvisioning(); // support scim provisioning

var app = builder.Build();
app.UseRouting();
app.UseWebfinger();
app.UseFastFed()
    .UseIdentityProvider();
app.MapControllerRoute(
    name: "defaultWithArea",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();