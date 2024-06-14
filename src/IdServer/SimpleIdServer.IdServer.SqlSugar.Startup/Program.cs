// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using NeoSmart.Caching.Sqlite.AspNetCore;
using SimpleIdServer.Configuration;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.SqlSugar.Startup;
using SimpleIdServer.IdServer.SqlSugar.Startup.Configurations;
using SimpleIdServer.IdServer.SqlSugar.Startup.Converters;
using SimpleIdServer.IdServer.Store.SqlSugar;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SimpleIdServer.IdServer.WsFederation;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

const string SQLServerCreateTableFormat = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DistributedCache' and xtype='U') " +
    "CREATE TABLE [dbo].[DistributedCache] (" +
    "Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL, " +
    "Value varbinary(MAX) NOT NULL, " +
    "ExpiresAtTime datetimeoffset NOT NULL, " +
    "SlidingExpirationInSeconds bigint NULL," +
    "AbsoluteExpiration datetimeoffset NULL, " +
    "PRIMARY KEY (Id))";

const string MYSQLCreateTableFormat =
            "CREATE TABLE IF NOT EXISTS DistributedCache (" +
                "`Id` varchar(449) CHARACTER SET ascii COLLATE ascii_bin NOT NULL," +
                "`AbsoluteExpiration` datetime(6) DEFAULT NULL," +
                "`ExpiresAtTime` datetime(6) NOT NULL," +
                "`SlidingExpirationInSeconds` bigint(20) DEFAULT NULL," +
                "`Value` longblob NOT NULL," +
                "PRIMARY KEY(`Id`)," +
                "KEY `Index_ExpiresAtTime` (`ExpiresAtTime`)" +
")";
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
// builder.Services.AddJsonSeeding(builder.Configuration);
// builder.Services.AddEntitySeeders(typeof(UserEntitySeeder));

var app = builder.Build();
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
SeedData(app);
app.Run();

void ConfigureIdServer(IServiceCollection services)
{
    var idServerBuilder = services.AddSIDIdentityServer(callback: cb =>
        {
            if (!string.IsNullOrWhiteSpace(identityServerConfiguration.SessionCookieNamePrefix))
                cb.SessionCookieName = identityServerConfiguration.SessionCookieNamePrefix;
            cb.Authority = identityServerConfiguration.Authority;
            cb.IsPasswordEncodeInBase64 = true;
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
        o.Add<WebauthnOptions>();
        o.Add<MobileOptions>();
        o.Add<IdServerConsoleOptions>();
        o.Add<GotifyOptions>();
        o.Add<IdServerVpOptions>();
        o.Add<UserLockingOptions>();
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

void SeedData(WebApplication webApplication)
{
    var dbContext = webApplication.Services.GetRequiredService<DbContext>();
    dbContext.Migrate();
    var transactionBuilder = webApplication.Services.GetRequiredService<ITransactionBuilder>();
    var realmRepository = webApplication.Services.GetRequiredService<IRealmRepository>();
    var scopeRepository = webApplication.Services.GetRequiredService<IScopeRepository>();
    var userRepository = webApplication.Services.GetRequiredService<IUserRepository>();
    var clientRepository = webApplication.Services.GetRequiredService<IClientRepository>();
    var umaPendingRequestRepository = webApplication.Services.GetRequiredService<IUmaPendingRequestRepository>();
    var umaResourceRepository = webApplication.Services.GetRequiredService<IUmaResourceRepository>();
    var gotifySessionRepository = webApplication.Services.GetRequiredService<IGotiySessionStore>();
    var languageRepository = webApplication.Services.GetRequiredService<ILanguageRepository>();
    var providerDefinitionRepository = webApplication.Services.GetRequiredService<IAuthenticationSchemeProviderDefinitionRepository>();
    var authSchemeProviderRepository = webApplication.Services.GetRequiredService<IAuthenticationSchemeProviderRepository>();
    var idProvisioningDefRepository = webApplication.Services.GetRequiredService<IIdentityProvisioningStore>();
    var registrationWorkflowRepository = webApplication.Services.GetRequiredService<IRegistrationWorkflowRepository>();
    var serializedFileKeyStore = webApplication.Services.GetRequiredService<IFileSerializedKeyStore>();
    var certificateAuthorityRepository = webApplication.Services.GetRequiredService<ICertificateAuthorityRepository>();
    var presentationDefinitionRepository = webApplication.Services.GetRequiredService<IPresentationDefinitionStore>();
    var acrRepository = webApplication.Services.GetRequiredService<IAuthenticationContextClassReferenceRepository>();
    var configurationDefinitionRepository = webApplication.Services.GetRequiredService<IConfigurationDefinitionStore>();
    using (var transaction = transactionBuilder.Build())
    {
        foreach (var realm in IdServerConfiguration.Realms)
            realmRepository.Add(realm);
        
        foreach(var scope in IdServerConfiguration.Scopes)
            scopeRepository.Add(scope);
        
        foreach (var user in IdServerConfiguration.Users)
            userRepository.Add(user);
        
        foreach (var client in IdServerConfiguration.Clients)
            clientRepository.Add(client);
        
        foreach (var umaPendingRequest in IdServerConfiguration.PendingRequests)
            umaPendingRequestRepository.Add(umaPendingRequest);
        
        foreach (var umaResource in IdServerConfiguration.Resources)
            umaResourceRepository.Add(umaResource);
        
        foreach (var gotifySession in IdServerConfiguration.Sessions)
            gotifySessionRepository.Add(gotifySession);
        
        foreach (var language in IdServerConfiguration.Languages)
            languageRepository.Add(language);
        
        foreach (var definition in IdServerConfiguration.ProviderDefinitions)
            providerDefinitionRepository.Add(definition);
        
        foreach (var authProvider in IdServerConfiguration.Providers)
            authSchemeProviderRepository.Add(authProvider);
        
        foreach (var idProvisioningDef in IdServerConfiguration.IdentityProvisioningDefLst)
            idProvisioningDefRepository.Add(idProvisioningDef);
        
        foreach (var registrationWorkflow in IdServerConfiguration.RegistrationWorkflows)
            registrationWorkflowRepository.Add(registrationWorkflow);

        serializedFileKeyStore.Add(KeyGenerator.GenerateRSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "rsa-1"));
        serializedFileKeyStore.Add(KeyGenerator.GenerateECDSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "ecdsa-1"));
        serializedFileKeyStore.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));

        foreach (var certificateAuthority in IdServerConfiguration.CertificateAuthorities)
            certificateAuthorityRepository.Add(certificateAuthority);

        foreach (var presentationDefinition in IdServerConfiguration.PresentationDefinitions)
            presentationDefinitionRepository.Add(presentationDefinition);

        acrRepository.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance);
        acrRepository.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.IapSilver);
        acrRepository.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
        {
            Id = Guid.NewGuid().ToString(),
            Name = "email",
            AuthenticationMethodReferences = new[] { "email" },
            DisplayName = "Email authentication",
            UpdateDateTime = DateTime.UtcNow,
            Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
            {
                SimpleIdServer.IdServer.Constants.StandardRealms.Master
            }
        });
        acrRepository.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
        {
            Id = Guid.NewGuid().ToString(),
            Name = "sms",
            AuthenticationMethodReferences = new[] { "sms" },
            DisplayName = "Sms authentication",
            UpdateDateTime = DateTime.UtcNow,
            Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
            {
                SimpleIdServer.IdServer.Constants.StandardRealms.Master
            }
        });
        acrRepository.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
        {
            Id = Guid.NewGuid().ToString(),
            Name = "pwd-email",
            AuthenticationMethodReferences = new[] { "pwd", "email" },
            DisplayName = "Password and email authentication",
            UpdateDateTime = DateTime.UtcNow,
            Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
            {
                SimpleIdServer.IdServer.Constants.StandardRealms.Master
            }
        });

        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<FacebookOptionsLite>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<LDAPRepresentationsExtractionJobOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<SCIMRepresentationsExtractionJobOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<IdServerEmailOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<IdServerSmsOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<IdServerPasswordOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<IdServerVpOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<WebauthnOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<MobileOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<IdServerConsoleOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<GotifyOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<GoogleOptionsLite>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<NegotiateOptionsLite>());
        configurationDefinitionRepository.Add(ConfigurationDefinitionExtractor.Extract<UserLockingOptions>());

        transaction.Commit(CancellationToken.None).Wait();
    }

    EnsureIsolationLevel();

    void EnsureIsolationLevel()
    {
        var ado = dbContext.SqlSugarClient.Ado.Connection;
        if (ado is SqlConnection)
        {
            dbContext.SqlSugarClient.Ado.ExecuteCommand("ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON");
            dbContext.SqlSugarClient.Ado.ExecuteCommand(SQLServerCreateTableFormat);
            return;
        }

        if(ado is MySqlConnection)
        {
            dbContext.SqlSugarClient.Ado.ExecuteCommand(MYSQLCreateTableFormat);
            return;
        }
    }
}
