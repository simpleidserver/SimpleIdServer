// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeoSmart.Caching.Sqlite.AspNetCore;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Seeding;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.SqlSugar.Startup;
using SimpleIdServer.IdServer.SqlSugar.Startup.Configurations;
using SimpleIdServer.IdServer.SqlSugar.Startup.Converters;
using SimpleIdServer.IdServer.Store.SqlSugar;
using SimpleIdServer.IdServer.Store.SqlSugar.Seeding;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net;

var storageTypes = new Dictionary<StorageTypes, DbType>
{
    { StorageTypes.SQLSERVER, DbType.SqlServer },
    { StorageTypes.POSTGRE, DbType.PostgreSQL },
    { StorageTypes.SQLITE, DbType.Sqlite },
    { StorageTypes.MYSQL, DbType.MySql }
};
ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
var identityServerConfiguration = builder.Configuration.Get<IdentityServerConfiguration>();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(o =>
    {
        o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        if (identityServerConfiguration.ClientCertificateMode != null) o.ClientCertificateMode = identityServerConfiguration.ClientCertificateMode.Value;
    });
});
if (identityServerConfiguration.IsForwardedEnabled)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}


var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
var conf = section.Get<StorageConfiguration>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddLocalization();
ConfigureIdServer(builder.Services);
ConfigureCentralizedConfiguration(builder);

// Uncomment these two lines to enable seed data from JSON file.
builder.Services.AddJsonSeeding(builder.Configuration);
builder.Services.AddEntitySeeders(typeof(UserEntitySeeder));

var app = builder.Build();
SeedData(app, identityServerConfiguration.SCIMBaseUrl);
app.UseCors("AllowAll");
if (identityServerConfiguration.IsForwardedEnabled)
{
    app.UseForwardedHeaders();
}

if (identityServerConfiguration.ForceHttps)
    app.SetHttpsScheme();

app.UseRequestLocalization(e =>
{
    e.SetDefaultCulture("en");
    e.AddSupportedCultures("en");
    e.AddSupportedUICultures("en");
});

if (!app.Environment.IsDevelopment())
{
    var errorPath = identityServerConfiguration.IsRealmEnabled ? "/master/Error/Unexpected" : "/Error/Unexpected";
    app.UseExceptionHandler(errorPath);
}

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
    .UseAutomaticConfiguration();

app.Run();

void ConfigureIdServer(IServiceCollection services)
{
    var idServerBuilder = services.AddSIDIdentityServer(callback: cb =>
        {
            if (!string.IsNullOrWhiteSpace(identityServerConfiguration.SessionCookieNamePrefix))
                cb.SessionCookieName = identityServerConfiguration.SessionCookieNamePrefix;
            cb.Authority = identityServerConfiguration.Authority;
        }, cookie: c =>
        {
            if (!string.IsNullOrWhiteSpace(identityServerConfiguration.AuthCookieNamePrefix))
                c.Cookie.Name = identityServerConfiguration.AuthCookieNamePrefix;
        }, dataProtectionBuilderCallback: ConfigureDataProtection)
        .UseSqlSugar(o =>
        {
            o.ConnectionConfig = new ConnectionConfig { DbType = storageTypes[conf.Type], ConnectionString = conf.ConnectionString };
        })
        .AddSwagger(o =>
        {
            o.IncludeDocumentation<AccessTokenTypeService>();
            o.AddOAuthSecurity();
        })
        .AddConsoleNotification()
        .AddVpAuthentication()
        .UseInMemoryMassTransit()
        .AddBackChannelAuthentication()
        .AddPwdAuthentication()
        .AddEmailAuthentication()
        .AddOtpAuthentication()
        .AddSmsAuthentication()
        .AddFcmNotification()
        .AddGotifyNotification()
        .AddSamlIdp()
        .AddFidoAuthentication(f =>
        {
            var authority = identityServerConfiguration.Authority;
            var url = new Uri(authority);
            f.ServerName = "SimpleIdServer";
            f.ServerDomain = url.Host;
            f.Origins = new HashSet<string> { authority };
        })
        .EnableConfigurableAuthentication()
        .AddSCIMProvisioning()
        .AddLDAPProvisioning()
        .AddAuthentication(callback: (a) =>
        {
            a.AddMutualAuthentication(m =>
            {
                m.AllowedCertificateTypes = CertificateTypes.All;
                m.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
            });
        });
    var isRealmEnabled = identityServerConfiguration.IsRealmEnabled;
    if (isRealmEnabled) idServerBuilder.UseRealm();
    services.AddDidKey();
    ConfigureDistributedCache();
}

void ConfigureCentralizedConfiguration(WebApplicationBuilder builder)
{
    var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
    var conf = section.Get<StorageConfiguration>();
    builder.AddAutomaticConfiguration(o =>
    {
        o.Add<FacebookOptionsLite>();
        o.Add<GoogleOptionsLite>();
        o.Add<LDAPRepresentationsExtractionJobOptions>();
        o.Add<SCIMRepresentationsExtractionJobOptions>();
        o.Add<IdServerEmailOptions>();
        o.Add<IdServerSmsOptions>();
        o.Add<IdServerPasswordOptions>();
        o.Add<FidoOptions>();
        o.Add<IdServerConsoleOptions>();
        o.Add<GotifyOptions>();
        o.Add<IdServerVpOptions>();
        o.Add<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>();
        o.UseEFConnector();
    });
}

void ConfigureDistributedCache()
{
    var section = builder.Configuration.GetSection(nameof(DistributedCacheConfiguration));
    var conf = section.Get<DistributedCacheConfiguration>();
    switch (conf.Type)
    {
        case DistributedCacheTypes.SQLSERVER:
            builder.Services.AddDistributedSqlServerCache(opts =>
            {
                opts.ConnectionString = conf.ConnectionString;
                opts.SchemaName = "dbo";
                opts.TableName = "DistributedCache";
            });
            break;
        case DistributedCacheTypes.REDIS:
            builder.Services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = conf.ConnectionString;
                opts.InstanceName = conf.InstanceName;
            });
            break;
        case DistributedCacheTypes.POSTGRE:
            builder.Services.AddDistributedPostgreSqlCache(opts =>
            {
                opts.ConnectionString = conf.ConnectionString;
                opts.SchemaName = "public";
                opts.TableName = "DistributedCache";
            });
            break;
        case DistributedCacheTypes.MYSQL:
            builder.Services.AddDistributedMySqlCache(opts =>
            {
                opts.ConnectionString = conf.ConnectionString;
                opts.SchemaName = "idserver";
                opts.TableName = "DistributedCache";
            });
            break;
        case DistributedCacheTypes.SQLITE:
            // Note : we cannot use the same database, because the library Neosmart, checks if the database contains only two tables and one index.
            builder.Services.AddSqliteCache(options =>
            {
                options.CachePath = conf.ConnectionString;
            });
            break;
    }
}

void ConfigureDataProtection(IDataProtectionBuilder dataProtectionBuilder)
{
    dataProtectionBuilder.Services.PersistKeysToSqlSugar();
}

void SeedData(WebApplication application, string scimBaseUrl)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<DbContext>())
        {
            // Uncomment these two lines to enable seed data from an external resource like JSON file.
            ISeedStrategy seedingService = scope.ServiceProvider.GetService<ISeedStrategy>();
            seedingService.SeedDataAsync().Wait();
        }
    }
}