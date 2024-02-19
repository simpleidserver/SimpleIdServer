// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Configuration;
using SimpleIdServer.Configuration.Redis;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Startup.Configurations;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Startup.Services;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

const string CreateTableFormat = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DistributedCache' and xtype='U') " +
    "CREATE TABLE [dbo].[DistributedCache] (" +
    "Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL, " +
    "Value varbinary(MAX) NOT NULL, " +
    "ExpiresAtTime datetimeoffset NOT NULL, " +
    "SlidingExpirationInSeconds bigint NULL," +
    "AbsoluteExpiration datetimeoffset NULL, " +
    "PRIMARY KEY (Id))";

ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(o =>
    {
        o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
    });
});

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
ConfigureCentralizedConfiguration(builder);
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
ConfigureIdServer(builder.Services);
var app = builder.Build();
SeedData(app, builder.Configuration["SCIMBaseUrl"]);
app.UseCors("AllowAll");
app.UseSID()
    .UseAutomaticConfiguration();
app.Run();

void ConfigureIdServer(IServiceCollection services)
{
    services.AddSIDIdentityServer(dataProtectionBuilderCallback: ConfigureDataProtection)
        .UseEFStore(o => ConfigureStorage(o))
        .UseInMemoryMassTransit()
        .AddBackChannelAuthentication()
        .AddPwdAuthentication()
        .EnableConfigurableAuthentication()
        .UseRealm()
        .AddAuthentication(callback: (a) =>
        {
            a.AddMutualAuthentication(m =>
            {
                m.AllowedCertificateTypes = CertificateTypes.All;
                m.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
            });
        });
    ConfigureDistributedCache();
    ConfigureApiAuthentication(services);
}

void ConfigureApiAuthentication(IServiceCollection services)
{
    var pwdAuthService = services.First(s => s.ServiceType == typeof(IPasswordAuthenticationService));
    var userRepository = services.First(s => s.ServiceType == typeof(IUserRepository));
    services.Remove(pwdAuthService);
    services.Remove(userRepository);
    var t = typeof(SimpleIdServer.IdServer.UI.HomeController);
    services.AddTransient<IAuthenticationMethodService, PwdAuthenticationMethodService>();
    services.AddTransient<IUserAuthenticationService, CustomPasswordAuthenticationService>();
    services.AddTransient<IPasswordAuthenticationService, CustomPasswordAuthenticationService>();
    services.AddTransient<IUserRepository, CustomUserRepository>();
    services.Configure<UserApiOptions>(o =>
    {
        o.BaseUrl = "http://localhost:53060";
    });
}

void ConfigureCentralizedConfiguration(WebApplicationBuilder builder)
{
    var section = builder.Configuration.GetSection(nameof(DistributedCacheConfiguration));
    var conf = section.Get<DistributedCacheConfiguration>();
    builder.AddAutomaticConfiguration(o =>
    {
        o.Add<FacebookOptionsLite>();
        if(conf.Type == DistributedCacheTypes.REDIS)
        {
            o.UseRedisConnector(conf.ConnectionString);
        }
        else
        {
            o.UseEFConnector(b =>
            {
                switch (conf.Type)
                {
                    case DistributedCacheTypes.INMEMORY:
                        b.UseInMemoryDatabase(conf.ConnectionString);
                        break;
                    case DistributedCacheTypes.SQLSERVER:
                        b.UseSqlServer(conf.ConnectionString);
                        break;
                }
            });
        }
    });
}

void ConfigureDistributedCache()
{
    var section = builder.Configuration.GetSection(nameof(DistributedCacheConfiguration));
    var conf = section.Get<DistributedCacheConfiguration>();
    switch(conf.Type)
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
    }
}

void ConfigureStorage(DbContextOptionsBuilder b)
{
    var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
    var conf = section.Get<StorageConfiguration>();
    switch(conf.Type) 
    {
        case StorageTypes.SQLSERVER:
            b.UseSqlServer(conf.ConnectionString, o =>
            {
                o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            break;
        case StorageTypes.POSTGRE:
            b.UseNpgsql(conf.ConnectionString, o =>
            {
                o.MigrationsAssembly("SimpleIdServer.IdServer.PostgreMigrations");
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            break;
        case StorageTypes.INMEMORY:
            b.UseInMemoryDatabase(conf.ConnectionString);
            break;
    }
}

void ConfigureDataProtection(IDataProtectionBuilder dataProtectionBuilder)
{
    dataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
}

void SeedData(WebApplication application, string scimBaseUrl)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
        {
            var isInMemory = dbContext.Database.IsInMemory();
            if (!isInMemory) dbContext.Database.Migrate();
            if (!dbContext.Realms.Any())
                dbContext.Realms.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Realms);

            if (!dbContext.Scopes.Any())
                dbContext.Scopes.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Scopes);

            if (!dbContext.Users.Any())
                dbContext.Users.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Users);

            if (!dbContext.Clients.Any())
                dbContext.Clients.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients);

            if (!dbContext.UmaPendingRequest.Any())
                dbContext.UmaPendingRequest.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.PendingRequests);

            if (!dbContext.UmaResources.Any())
                dbContext.UmaResources.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Resources);

            if (!dbContext.AuthenticationSchemeProviderDefinitions.Any())
                dbContext.AuthenticationSchemeProviderDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.ProviderDefinitions);

            if (!dbContext.AuthenticationSchemeProviders.Any())
                dbContext.AuthenticationSchemeProviders.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Providers);

            if (!dbContext.RegistrationWorkflows.Any())
                dbContext.RegistrationWorkflows.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.RegistrationWorkflows);

            if (!dbContext.SerializedFileKeys.Any())
            {
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateRSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "rsa-1"));
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateECDSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "ecdsa-1"));
            }

            if(!dbContext.CertificateAuthorities.Any())
                dbContext.CertificateAuthorities.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.CertificateAuthorities);

            if (!dbContext.Acrs.Any())
            {
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance);
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.IapSilver);
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "email",
                    AuthenticationMethodReferences = new[] { "email" },
                    DisplayName = "Email authentication",
                    UpdateDateTime = DateTime.UtcNow,
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "sms",
                    AuthenticationMethodReferences = new[] { "sms" },
                    DisplayName = "Sms authentication",
                    UpdateDateTime = DateTime.UtcNow,
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "pwd-email",
                    AuthenticationMethodReferences = new[] { "pwd", "email" },
                    DisplayName = "Password and email authentication",
                    UpdateDateTime = DateTime.UtcNow,
                    Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
                });
            }

            if(!dbContext.Definitions.Any())
            {
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<FacebookOptionsLite>());
            }

            if(!isInMemory)
            {
                var dbConnection = dbContext.Database.GetDbConnection() as SqlConnection;
                if (dbConnection != null)
                {
                    if (dbConnection.State != System.Data.ConnectionState.Open) dbConnection.Open();
                    var cmd = dbConnection.CreateCommand();
                    cmd.CommandText = "ALTER DATABASE IdServer SET ALLOW_SNAPSHOT_ISOLATION ON";
                    cmd.ExecuteNonQuery();
                    cmd = dbConnection.CreateCommand();
                    cmd.CommandText = CreateTableFormat;
                    cmd.ExecuteNonQuery();
                }
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