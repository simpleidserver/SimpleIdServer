﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeoSmart.Caching.Sqlite.AspNetCore;
using SimpleIdServer.Configuration;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Startup.Configurations;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SimpleIdServer.IdServer.WsFederation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
    .UseAutomaticConfiguration()
    .UseOpenidFederation();

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
        .UseEFStore(o => ConfigureStorage(o))
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
        })
        .AddOpenidFederation(o =>
        {
            o.IsFederationEnabled = true;
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
        case StorageTypes.SQLITE:
            b.UseSqlite(conf.ConnectionString, o =>
            {
                o.MigrationsAssembly("SimpleIdServer.IdServer.SqliteMigrations");
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            // SQLITE creates an ambient transaction.
            b.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            break;
    }
}

void ConfigureDataProtection(IDataProtectionBuilder dataProtectionBuilder)
{
    dataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
}

async void SeedData(WebApplication application, string scimBaseUrl)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
        {
            var isInMemory = dbContext.Database.IsInMemory();
            if (!isInMemory) dbContext.Database.Migrate();

            var masterRealm = dbContext.Realms.FirstOrDefault(r => r.Name == SimpleIdServer.IdServer.Constants.StandardRealms.Master.Name) ?? SimpleIdServer.IdServer.Constants.StandardRealms.Master;
            if (!dbContext.Realms.Any())
                dbContext.Realms.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Realms);
            var unsupportedScopes = MigrateScopes(dbContext, masterRealm);
            MigrateClients(dbContext, unsupportedScopes);
            if (!dbContext.UmaPendingRequest.Any())
                dbContext.UmaPendingRequest.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.PendingRequests);

            if (!dbContext.UmaResources.Any())
                dbContext.UmaResources.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Resources);

            if (!dbContext.GotifySessions.Any())
                dbContext.GotifySessions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Sessions);

            foreach (var language in SimpleIdServer.IdServer.Startup.IdServerConfiguration.Languages)
                AddMissingLanguage(dbContext, language);

            foreach (var providerDefinition in SimpleIdServer.IdServer.Startup.IdServerConfiguration.ProviderDefinitions)
            {
                if (!dbContext.AuthenticationSchemeProviderDefinitions.Any(d => d.Name == providerDefinition.Name))
                {
                    dbContext.AuthenticationSchemeProviderDefinitions.Add(providerDefinition);
                }
            }

            AddMissingAuthenticationSchemeProviders(dbContext);
            if (!dbContext.IdentityProvisioningDefinitions.Any())
                dbContext.IdentityProvisioningDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.IdentityProvisioningDefLst);

            if (!dbContext.IdentityProvisioningLst.Any())
                dbContext.IdentityProvisioningLst.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.GetIdentityProvisiongLst(scimBaseUrl));

            if (!dbContext.RegistrationWorkflows.Any())
                dbContext.RegistrationWorkflows.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.RegistrationWorkflows);

            var groups = MigrateGroups(dbContext, masterRealm);
            MigrateUsers(dbContext, groups.adminGroup, groups.adminRoGroup);
            if (!dbContext.SerializedFileKeys.Any())
            {
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateRSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "rsa-1"));
                dbContext.SerializedFileKeys.Add(KeyGenerator.GenerateECDSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "ecdsa-1"));
                dbContext.SerializedFileKeys.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));
            }

            if (!dbContext.CertificateAuthorities.Any())
                dbContext.CertificateAuthorities.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.CertificateAuthorities);

            if (!dbContext.PresentationDefinitions.Any())
                dbContext.PresentationDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.PresentationDefinitions);

            if(!dbContext.FederationEntities.Any())
                dbContext.FederationEntities.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.FederationEntities);

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

            AddMissingConfigurationDefinition<FacebookOptionsLite>(dbContext);
            AddMissingConfigurationDefinition<LDAPRepresentationsExtractionJobOptions>(dbContext);
            AddMissingConfigurationDefinition<SCIMRepresentationsExtractionJobOptions>(dbContext);
            AddMissingConfigurationDefinition<IdServerEmailOptions>(dbContext);
            AddMissingConfigurationDefinition<IdServerSmsOptions>(dbContext);
            AddMissingConfigurationDefinition<IdServerPasswordOptions>(dbContext);
            AddMissingConfigurationDefinition<IdServerVpOptions>(dbContext);
            AddMissingConfigurationDefinition<WebauthnOptions>(dbContext);
            AddMissingConfigurationDefinition<MobileOptions>(dbContext);
            AddMissingConfigurationDefinition<IdServerConsoleOptions>(dbContext);
            AddMissingConfigurationDefinition<GotifyOptions>(dbContext);
            AddMissingConfigurationDefinition<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>(dbContext);
            AddMissingConfigurationDefinition<GoogleOptionsLite>(dbContext);
            AddMissingConfigurationDefinition<NegotiateOptionsLite>(dbContext);
            AddMissingConfigurationDefinition<UserLockingOptions>(dbContext);
            EnableIsolationLevel(dbContext);
            dbContext.SaveChanges();

            // Uncomment these two lines to enable seed data from an external resource like JSON file.
            // ISeedStrategy seedingService = scope.ServiceProvider.GetService<ISeedStrategy>();
            // seedingService.SeedDataAsync().Wait();
        }

        void EnableIsolationLevel(StoreDbContext dbContext)
        {
            if (dbContext.Database.IsInMemory()) return;
            var dbConnection = dbContext.Database.GetDbConnection();
            var sqlConnection = dbConnection as SqlConnection;
            if (sqlConnection != null)
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open) sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();
                cmd.CommandText = "ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON";
                cmd.ExecuteNonQuery();
                cmd = sqlConnection.CreateCommand();
                cmd.CommandText = SQLServerCreateTableFormat;
                cmd.ExecuteNonQuery();
                return;
            }

            var mysqlConnection = dbConnection as MySqlConnector.MySqlConnection;
            if (mysqlConnection != null)
            {
                if (mysqlConnection.State != System.Data.ConnectionState.Open) mysqlConnection.Open();
                var cmd = mysqlConnection.CreateCommand();
                cmd.CommandText = MYSQLCreateTableFormat;
                cmd.ExecuteNonQuery();
                return;
            }
        }

        void AddMissingConfigurationDefinition<T>(StoreDbContext dbContext)
        {
            var name = typeof(T).Name;
            if (!dbContext.Definitions.Any(d => d.Id == name))
            {
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<T>());
            }
        }

        void AddMissingLanguage(StoreDbContext dbContext, Language language)
        {
            if (!dbContext.Languages.Any(l => l.Code == language.Code))
                dbContext.Languages.Add(language);
            var keys = language.Descriptions.Select(d => d.Key);
            var existingTranslations = dbContext.Translations.Where(t => keys.Contains(t.Key)).ToList();
            var unknownTranslations = language.Descriptions.Where(d => !existingTranslations.Any(t => t.Key == d.Key && t.Language == d.Language));
            dbContext.Translations.AddRange(unknownTranslations);
            foreach (var existingTranslation in existingTranslations)
            {
                var tr = language.Descriptions.SingleOrDefault(d => d.Key == existingTranslation.Key && d.Language == existingTranslation.Language);
                if (tr == null) continue;
                existingTranslation.Value = tr.Value;
            }
        }

        void AddMissingAuthenticationSchemeProviders(StoreDbContext dbContext)
        {
            if (!dbContext.AuthenticationSchemeProviders.Any())
            {
                foreach (var provider in SimpleIdServer.IdServer.Startup.IdServerConfiguration.Providers)
                {
                    var def = dbContext.AuthenticationSchemeProviderDefinitions.FirstOrDefault(d => d.Name == provider.AuthSchemeProviderDefinition.Name);
                    if (def != null) provider.AuthSchemeProviderDefinition = def;
                    var realmName = provider.Realms.First().Name;
                    var realm = dbContext.Realms.FirstOrDefault(r => r.Name == realmName);
                    if (realm != null)
                    {
                        provider.Realms.Clear();
                        provider.Realms.Add(realm);
                    }

                    dbContext.AuthenticationSchemeProviders.Add(provider);
                }
            }
        }

        List<Scope> MigrateScopes(StoreDbContext dbContext, Realm masterRealm)
        {
            var allScopeNames = dbContext.Scopes.Select(s => s.Name);
            var unsupportedScopes = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Scopes.Where(s => !allScopeNames.Contains(s.Name));
            foreach (var scope in unsupportedScopes)
                scope.Realms = new List<Realm>
                {
                    masterRealm
                };
            dbContext.Scopes.AddRange(unsupportedScopes);
            return unsupportedScopes.ToList();
        }

        void MigrateClients(StoreDbContext dbContext, List<Scope> unsupportedScopes)
        {
            var confClientIds = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Select(c => c.ClientId);
            var allClientIds = dbContext.Clients.Select(s => s.ClientId);
            var unknownClients = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Where(c => !allClientIds.Contains(c.ClientId));
            var knownClients = dbContext.Clients
                .Include(c => c.Scopes)
                .Where(c => confClientIds.Contains(c.ClientId));
            foreach (var unknownClient in unknownClients)
                dbContext.Clients.Add(unknownClient);
            foreach (var knownClient in knownClients)
            {
                var cl = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Single(c => c.ClientId == knownClient.ClientId);
                foreach (var scope in cl.Scopes)
                {
                    if(!knownClient.Scopes.Any(s => s.Name == scope.Name))
                    {
                        var existingScope = dbContext.Scopes.SingleOrDefault(s => s.Name == scope.Name) ?? unsupportedScopes.Single(s => s.Name == scope.Name);
                        knownClient.Scopes.Add(existingScope);
                    }
                }
            }
        }

        (Group adminGroup, Group adminRoGroup) MigrateGroups(StoreDbContext dbContext, Realm masterRealm)
        {
            var admGroup = dbContext.Groups.Include(g => g.Realms)
                .Include(g => g.Roles)
                .FirstOrDefault(g => g.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup.Name);
            var admRoGroup = dbContext.Groups.Include(g => g.Realms)
                .Include(g => g.Roles)
                .FirstOrDefault(g => g.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup.Name);
            if (admGroup == null)
            {
                admGroup = SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup;
                admGroup.Realms = new List<GroupRealm>();
                masterRealm.Groups.Add(new GroupRealm
                {
                    Group = admGroup
                });
            }

            if (admRoGroup == null)
            {
                admRoGroup = SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup;
                admRoGroup.Realms = new List<GroupRealm>();
                masterRealm.Groups.Add(new GroupRealm
                {
                    Group = admRoGroup
                });
            }

            var scopes = RealmRoleBuilder.BuildAdministrativeRole(masterRealm);
            var allScopeNames = dbContext.Scopes.Select(s => s.Name);
            var unknownScopes = scopes.Where(s => !allScopeNames.Contains(s.Name));
            dbContext.Scopes.AddRange(unknownScopes);
            foreach (var scope in unknownScopes)
            {
                if (!admGroup.Roles.Any(r => r.Name == scope.Name))
                    admGroup.Roles.Add(scope);
            }

            foreach(var scope in unknownScopes.Where(s => s.Action == ComponentActions.View))
            {
                if (!admRoGroup.Roles.Any(r => r.Name == scope.Name))
                    admRoGroup.Roles.Add(scope);
            }

            var existingAdministratorRole = dbContext.Scopes.FirstOrDefault(s => s.Name == SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole.Name);
            if (existingAdministratorRole == null)
            {
                existingAdministratorRole = SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole;
                existingAdministratorRole.Realms.Clear();
                existingAdministratorRole.Realms.Add(masterRealm);
                dbContext.Scopes.Add(existingAdministratorRole);
            }

            if (!admGroup.Roles.Any(r => r.Name == SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole.Name))
                admGroup.Roles.Add(existingAdministratorRole);

            return (admGroup, admRoGroup);
        }

        void MigrateUsers(StoreDbContext dbContext, Group adminGroup, Group adminRoGroup)
        {
            var isUserExists = dbContext.Users
                .Any(c => c.Name == "user");
            var existingAdministratorUser = dbContext.Users
                .Include(u => u.Groups).ThenInclude(u => u.Group)
                .FirstOrDefault(u => u.Name == SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorUser.Name);
            var existingAdministratorRoUser = dbContext.Users
                .Include(u => u.Groups).ThenInclude(u => u.Group)
                .FirstOrDefault(u => u.Name == SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorReadonlyUser.Name);
            if(!isUserExists)
                dbContext.Users.Add(UserBuilder.Create("user", "password", "User").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").Build());
            if (existingAdministratorRoUser == null)
                dbContext.Users.Add(SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorReadonlyUser);
            else if(!existingAdministratorRoUser.Groups.Any(g => g.Group.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup.Name))
            {
                existingAdministratorRoUser.Groups.Add(new GroupUser
                {
                    Group = adminRoGroup
                });
            }

            if (existingAdministratorUser == null)
                dbContext.Users.Add(SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorUser);
            else if(!existingAdministratorUser.Groups.Any(g => g.Group.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup.Name))
            {
                existingAdministratorUser.Groups.Add(new GroupUser
                {
                    Group = adminGroup
                });
            }
        }
    }
}