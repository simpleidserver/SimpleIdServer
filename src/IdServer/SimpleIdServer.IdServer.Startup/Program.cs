// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.WsFederation;
using System;
using System.Collections.Generic;
using System.Linq;
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
SeedData(app);
app.UseCors("AllowAll");
app.UseSID()
    .UseWsFederation();
app.Run();

void RunSqlServerIdServer(IServiceCollection services)
{
    var name = Assembly.GetExecutingAssembly().GetName().Name;
    services.AddSIDIdentityServer()
        .UseEFStore(o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("IdServer"), o =>
            {
                o.MigrationsAssembly(name);
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        })
        .UseInMemoryMassTransit()
        .AddBackChannelAuthentication()
        .AddEmailAuthentication()
        .AddSmsAuthentication()
        .EnableConfigurableAuthentication()
        .UseRealm()
        .AddAuthentication(callback: (a) =>
        {
            /*
            a.AddWsAuthentication(o =>
            {
                o.MetadataAddress = "http://localhost:5001";
                o.Wtrealm = "urn:website";
                o.RequireHttpsMetadata = false;
            });
            */
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

void SeedData(WebApplication application)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
        {
            dbContext.Database.Migrate();
            if (!dbContext.Realms.Any())
                dbContext.Realms.AddRange(IdServerConfiguration.Realms);

            if (!dbContext.Scopes.Any())
                dbContext.Scopes.AddRange(IdServerConfiguration.Scopes);

            if (!dbContext.Users.Any())
                dbContext.Users.AddRange(IdServerConfiguration.Users);

            if (!dbContext.Clients.Any())
                dbContext.Clients.AddRange(IdServerConfiguration.Clients);

            if (!dbContext.UmaPendingRequest.Any())
                dbContext.UmaPendingRequest.AddRange(IdServerConfiguration.PendingRequests);

            if (!dbContext.UmaResources.Any())
                dbContext.UmaResources.AddRange(IdServerConfiguration.Resources);

            if (!dbContext.AuthenticationSchemeProviderDefinitions.Any())
                dbContext.AuthenticationSchemeProviderDefinitions.AddRange(IdServerConfiguration.ProviderDefinitions);

            if (!dbContext.AuthenticationSchemeProviders.Any())
                dbContext.AuthenticationSchemeProviders.AddRange(IdServerConfiguration.Providers);

            if(!dbContext.SerializedFileKeys.Any())
            {
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));
                dbContext.SerializedFileKeys.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));
            }

            if (!dbContext.Acrs.Any())
            {
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance);
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.IapSilver);
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "email",
                    AuthenticationMethodReferences = new[] { "email" },
                    DisplayName = "Email authentication",
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "sms",
                    AuthenticationMethodReferences = new[] { "sms" },
                    DisplayName = "Sms authentication",
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "pwd-email",
                    AuthenticationMethodReferences = new[] { "pwd", "email" },
                    DisplayName = "Password and email authentication",
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
            }

            dbContext.SaveChanges();
        }
    }
}

static RsaSecurityKey BuildRsaSecurityKey(string keyid) => new RsaSecurityKey(RSA.Create())
{
    KeyId = keyid
};

static ECDsaSecurityKey BuildECDSaSecurityKey(ECCurve curve) => new ECDsaSecurityKey(ECDsa.Create(curve))
{
    KeyId = Guid.NewGuid().ToString()
};