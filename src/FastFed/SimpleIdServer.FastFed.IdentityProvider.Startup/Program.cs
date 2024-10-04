// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.IdentityProvider.Options;
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;
using SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml;
using SimpleIdServer.FastFed.IdentityProvider.Startup.Configurations;
using SimpleIdServer.FastFed.Store.EF;
using SimpleIdServer.Webfinger;
using SimpleIdServer.Webfinger.Builders;
using System.Collections.Generic;
using System.Security.Cryptography;
using SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml.Sid;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var authSection = builder.Configuration.GetSection(nameof(AuthOptions));
var authOptions = authSection.Get<AuthOptions>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddControllersWithViews();

builder.Services.AddWebfinger()
    .UseInMemoryEfStore(WebfingerResourceBuilder.New("acct", $"jane@{builder.Configuration["ProviderDomain"]}").AddLinkRelation("https://openid.net/specs/fastfed/1.0/provider", "https://localhost:5020/fastfed").Build());
var fastFedBuilder = builder.Services.AddFastFed(o =>
{
    o.ProviderDomain = builder.Configuration["ProviderDomain"];
    o.IdProvider = new SimpleIdServer.FastFed.IdProviderOptions
    {
        Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
        {
            ProvisioningProfiles = new List<string>
            {
                "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise",
                "urn:ietf:params:fastfed:1.0:provisioning:saml:2.0:enterprise"
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
        },
        JwksUri = "https://localhost:5020/jwks"
    };
})
    .AddFastFedIdentityProvider(cbChooser: (t) => t.UseInMemoryEfStore(), (cb) =>
    {
        cb.SigningCredentials = new List<Microsoft.IdentityModel.Tokens.SigningCredentials>
        {
            GenerateRSASignatureKey("keyId")
        };
    }) // support scim provisioning
    .AddIdProviderScimProvisioning()
    .AddIdProviderSamlAuthentication(o =>
    {
        o.SamlMetadataUri = "https://openid.simpleidserver.com/master/saml/metadata";
    })
    .AddSidSamlAuthentication(o =>
    {
        o.ClientId = "fastFed";
        o.ClientSecret = "password";
        o.SidBaseUrl = "https://localhost:5001/master";
    })
    .UseDefaultIdProviderSecurity(authOptions);
ConfigureMessageBroker(fastFedBuilder);

void ConfigureMessageBroker(FastFedServicesBuilder fastFedBuilder)
{
    var section = builder.Configuration.GetSection(nameof(MessageBrokerOptions));
    var conf = section.Get<MessageBrokerOptions>();
    switch(conf.Transport)
    {
        case TransportTypes.SQLSERVER:
            builder.Services.AddOptions<SqlTransportOptions>()
                .Configure(options =>
                {
                    options.ConnectionString = conf.ConnectionString;
                    options.Username = conf.Username;
                    options.Password = conf.Password;
                });
            fastFedBuilder.AddSidScimProvisioning(o =>
            {
                o.UsingSqlServer((ctx, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            break;
        default:
            fastFedBuilder.AddSidScimProvisioning();
            break;
    }
}

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();
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

static SigningCredentials GenerateRSASignatureKey(string keyId, string alg = SecurityAlgorithms.RsaSha256)
{
    var rsa = RSA.Create();
    var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
    return new SigningCredentials(securityKey, alg);
}