// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Configuration;
using SimpleIdServer.Configuration.Redis;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.LDAP.Jobs;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Provisioning.SCIM.Jobs;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Startup.Configurations;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.WsFederation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
ConfigureCentralizedConfiguration(builder);
if (identityServerConfiguration.IsForwardedEnabled)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
ConfigureIdServer(builder.Services);

var app = builder.Build();
SeedData(app, identityServerConfiguration.SCIMBaseUrl);
app.UseCors("AllowAll");
if (identityServerConfiguration.IsForwardedEnabled)
{
    app.UseForwardedHeaders();
}

if (identityServerConfiguration.ForceHttps)
    app.SetHttpsScheme();

app
    .UseSID()
    .UseSIDSwagger()
    .UseSIDSwaggerUI()
    // .UseSIDReDoc()
    .UseWsFederation()
    .UseFIDO()
    .UseCredentialIssuer()
    .UseSamlIdp()
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
            if(!string.IsNullOrWhiteSpace(identityServerConfiguration.AuthCookieNamePrefix)) 
                c.Cookie.Name = identityServerConfiguration.AuthCookieNamePrefix;
        }, dataProtectionBuilderCallback: ConfigureDataProtection)
        .UseEFStore(o => ConfigureStorage(o))
        .AddSwagger(o =>
        {
            o.IncludeDocumentation<AccessTokenTypeService>();
            o.AddOAuthSecurity();
        })
        .AddCredentialIssuer()
        .UseInMemoryMassTransit()
        .AddBackChannelAuthentication()
        .AddPwdAuthentication()
        .AddEmailAuthentication()
        .AddSmsAuthentication()
        .AddFcmNotification()
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
                opts.Authority = identityServerConfiguration.Authority;
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
    var isRealmEnabled = identityServerConfiguration.IsRealmEnabled;
    if (isRealmEnabled) idServerBuilder.UseRealm();
    services.AddDIDKey();
    services.AddDIDEthr();
    ConfigureDistributedCache();
}

void ConfigureCentralizedConfiguration(WebApplicationBuilder builder)
{
    var section = builder.Configuration.GetSection(nameof(DistributedCacheConfiguration));
    var conf = section.Get<DistributedCacheConfiguration>();
    builder.AddAutomaticConfiguration(o =>
    {
        o.Add<FacebookOptionsLite>();
        o.Add<LDAPRepresentationsExtractionJobOptions>();
        o.Add<SCIMRepresentationsExtractionJobOptions>();
        o.Add<IdServerEmailOptions>();
        o.Add<IdServerSmsOptions>();
        o.Add<IdServerPasswordOptions>();
        o.Add<FidoOptions>();
        o.Add<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>();
        if (conf.Type == DistributedCacheTypes.REDIS)
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
                    case DistributedCacheTypes.POSTGRE:
                        b.UseNpgsql(conf.ConnectionString);
                        break;
                    case DistributedCacheTypes.MYSQL:
                        b.UseMySql(conf.ConnectionString, ServerVersion.AutoDetect(conf.ConnectionString));
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
                opts.TableName = "DistributedCache";
            });
            break;
    }
}

void ConfigureStorage(DbContextOptionsBuilder b)
{
    var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
    var conf = section.Get<StorageConfiguration>();
    switch (conf.Type)
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
        case StorageTypes.MYSQL:
            b.UseMySql(conf.ConnectionString, ServerVersion.AutoDetect(conf.ConnectionString), o =>
            {
                o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
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

            if (!dbContext.IdentityProvisioningDefinitions.Any())
                dbContext.IdentityProvisioningDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.IdentityProvisioningDefLst);

            if (!dbContext.IdentityProvisioningLst.Any())
                dbContext.IdentityProvisioningLst.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.GetIdentityProvisiongLst(scimBaseUrl));

            if (!dbContext.RegistrationWorkflows.Any())
                dbContext.RegistrationWorkflows.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.RegistrationWorkflows);

            if (!dbContext.SerializedFileKeys.Any())
            {
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateRSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "rsa-1"));
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateECDSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "ecdsa-1"));
                dbContext.SerializedFileKeys.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));
            }

            if (!dbContext.CertificateAuthorities.Any())
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

            if (!dbContext.Networks.Any())
                dbContext.Networks.Add(new SimpleIdServer.IdServer.Domains.NetworkConfiguration
                {
                    Name = "sepolia",
                    RpcUrl = "https://rpc.sepolia.org",
                    ContractAdr = SimpleIdServer.Did.Ethr.Constants.DefaultContractAdr,
                    PrivateAccountKey = "0fda34d0029c91481b1f54b0b68efea94c4572c80b2902cb3a2ab722b41fc1e1",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow
                });

            if (!dbContext.Definitions.Any())
            {
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<FacebookOptionsLite>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<LDAPRepresentationsExtractionJobOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<SCIMRepresentationsExtractionJobOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<IdServerEmailOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<IdServerSmsOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<IdServerPasswordOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<FidoOptions>());
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>());
            }

            EnableIsolationLevel(dbContext);
            dbContext.SaveChanges();
        }

        void EnableIsolationLevel(StoreDbContext dbContext)
        {
            if (dbContext.Database.IsInMemory()) return;
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
    }
}