// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.ConformanceSuite.Startup;
using SimpleIdServer.IdServer.Sms;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(o =>
    {
        o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
    });
});
builder.Configuration.AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
RunSqlServerIdServer(builder.Services);
var app = builder.Build();
app.UseCors("AllowAll");
app.UseSID()
    .UseWsFederation();
app.Run();

void RunSqlServerIdServer(IServiceCollection services)
{
    var name = Assembly.GetExecutingAssembly().GetName().Name;
    services.AddSIDIdentityServer()
        .UseInMemoryStore(o =>
        {
            o.AddInMemoryRealms(IdServerConfiguration.Realms);
            o.AddInMemoryScopes(IdServerConfiguration.Scopes);
            o.AddInMemoryClients(IdServerConfiguration.Clients);
            o.AddInMemoryUsers(IdServerConfiguration.Users);
            o.AddInMemoryUMAResources(IdServerConfiguration.Resources);
            o.AddInMemoryUMAPendingRequests(IdServerConfiguration.PendingRequests);
            o.AddInMemoryAuthenticationSchemeProviderDefinitions(IdServerConfiguration.ProviderDefinitions);
            o.AddInMemoryAuthenticationSchemeProviders(IdServerConfiguration.Providers);
            o.AddInMemoryKeys(SimpleIdServer.IdServer.Constants.StandardRealms.Master, new List<SigningCredentials>
            {
                new SigningCredentials(BuildRsaSecurityKey("rsaSig"), SecurityAlgorithms.RsaSha256),
                new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP256), SecurityAlgorithms.EcdsaSha256)
            }, new List<EncryptingCredentials>());

        })
        .UseInMemoryMassTransit()
        .AddBackChannelAuthentication()
        .AddEmailAuthentication()
        .AddSmsAuthentication()
        .EnableConfigurableAuthentication()
        .UseRealm()
        .AddAuthentication(callback: (a) =>
        {
            a.AddMutualAuthentication(m =>
            {
                m.AllowedCertificateTypes = CertificateTypes.All;
                m.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
            });
            a.AddOIDCAuthentication(opts =>
            {
                opts.Authority = "https://localhost.com:5001";
                opts.ClientId = "website";
                opts.ClientSecret = "password";
                opts.ResponseType = "code";
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
        });
}

static RsaSecurityKey BuildRsaSecurityKey(string keyid) => new RsaSecurityKey(RSA.Create())
{
    KeyId = keyid
};

static ECDsaSecurityKey BuildECDSaSecurityKey(ECCurve curve) => new ECDsaSecurityKey(ECDsa.Create(curve))
{
    KeyId = Guid.NewGuid().ToString()
};