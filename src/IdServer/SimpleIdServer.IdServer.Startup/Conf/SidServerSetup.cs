// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using EfdataSeeder;
using FormBuilder;
using Hangfire;
using Hangfire.MySql;
using Hangfire.PostgreSql;
using Hangfire.SQLite;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite.AspNetCore;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;
using SimpleIdServer.IdServer.Startup.Conf.Migrations.BeforeDeployment;
using SimpleIdServer.IdServer.Startup.Configurations;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace SimpleIdServer.IdServer.Startup.Conf;

public class SidServerSetup
{
    public static void ConfigureIdServer(WebApplicationBuilder builder, IdentityServerConfiguration configuration)
    {
        var services = builder.Services;
        var section = builder.Configuration.GetSection(nameof(ScimClientOptions));
        var conf = section.Get<ScimClientOptions>();
        services.AddServerSideBlazor().AddCircuitOptions(o =>
        {
            o.DetailedErrors = true;
        }).AddHubOptions(o =>
        {
            o.MaximumReceiveMessageSize = 102400000;
        });
        var idServerBuilder = services.AddSidIdentityServer(callback: cb =>
        {
            cb.DefaultAuthenticationWorkflowId = DataSeeder.completePwdAuthWorkflowId;
            if (!string.IsNullOrWhiteSpace(configuration.SessionCookieNamePrefix))
                cb.SessionCookieName = configuration.SessionCookieNamePrefix;
            cb.Authority = configuration.Authority;
            cb.ScimClientOptions = conf;
        }, cookie: c =>
        {
            if (!string.IsNullOrWhiteSpace(configuration.AuthCookieNamePrefix))
                c.Cookie.Name = configuration.AuthCookieNamePrefix;
        }, dataProtectionBuilderCallback: ConfigureDataProtection)
            .UseEFStore(o => ConfigureStorage(builder, o))
            .AddSwagger(o =>
            {
                o.IncludeDocumentation<AccessTokenTypeService>();
                o.AddOAuthSecurity();
            })
            .AddConsoleNotification()
            .AddVpAuthentication()
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
                var authority = configuration.Authority;
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
        var isRealmEnabled = configuration.IsRealmEnabled;
        if (isRealmEnabled) idServerBuilder.UseRealm();
        var cookieName = "XSFR-TOKEN";
        services.AddAntiforgery(c =>
        {
            c.Cookie.Name = cookieName;
        });
        services.AddFormBuilder(o =>
        {
            o.AntiforgeryCookieName = cookieName;
        }).UseEF((db) =>
        {
            ConfigureFormBuilderStorage(builder, db);
        });
        services.AddDidKey(o =>
        {
            o.PublicKeyFormat = Ed25519VerificationKey2020Standard.TYPE;
        });
        ConfigureDistributedCache(builder);
        ConfigureMessageBroker(builder, idServerBuilder);
        ConfigureHangfire(builder);
    }

    public static void ConfigureHangfire(WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        var services = builder.Services;
        services.AddHangfire(o => {
            o.UseRecommendedSerializerSettings();
            o.UseIgnoredAssemblyVersionTypeResolver();
            switch(conf.Type)
            {
                case StorageTypes.INMEMORY:
                    o.UseInMemoryStorage();
                    break;
                case StorageTypes.SQLSERVER:
                    o.UseSqlServerStorage(conf.ConnectionString);
                    break;
                case StorageTypes.POSTGRE:
                    o.UsePostgreSqlStorage(conf.ConnectionString);
                    break;
                case StorageTypes.MYSQL:
                    o.UseStorage(new MySqlStorage(conf.ConnectionString, new MySqlStorageOptions
                    {
                        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire"
                    }));
                    break;
                case StorageTypes.SQLITE:
                    o.UseSQLiteStorage(conf.ConnectionString);
                    break;
            }
        });
    }

    public static void ConfigureDataseeder(WebApplicationBuilder builder)
    {
        builder.Services.AddEfdataSeeder();
        builder.Services.AddTransient<IDataSeeder, FormAndWorkflowDataSeeder>();
        builder.Services.AddTransient<IDataSeeder, ClientTypeDataSeeder>();
    }

    public static void ConfigureCentralizedConfiguration(WebApplicationBuilder builder)
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

    private static void ConfigureDataProtection(IDataProtectionBuilder dataProtectionBuilder)
    {
        dataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
    }

    private static void ConfigureStorage(WebApplicationBuilder builder, DbContextOptionsBuilder b)
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
                b.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
                break;
        }
    }

    private static void ConfigureFormBuilderStorage(WebApplicationBuilder builder, DbContextOptionsBuilder b)
    {
        var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        switch (conf.Type)
        {
            case StorageTypes.SQLSERVER:
                b.UseSqlServer(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("FormBuilder.SqlServerMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.POSTGRE:
                b.UseNpgsql(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("FormBuilder.PostgreMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.MYSQL:
                b.UseMySql(conf.ConnectionString, ServerVersion.AutoDetect(conf.ConnectionString), o =>
                {
                    o.MigrationsAssembly("FormBuilder.MySQLMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.INMEMORY:
                b.UseInMemoryDatabase(conf.ConnectionString);
                break;
            case StorageTypes.SQLITE:
                b.UseSqlite(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("FormBuilder.SqliteMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                // SQLITE creates an ambient transaction.
                b.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
                break;
        }
    }

    private static void ConfigureDistributedCache(WebApplicationBuilder builder)
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

    private static void ConfigureMessageBroker(WebApplicationBuilder builder, IdServerBuilder idServerBuilder)
    {
        var section = builder.Configuration.GetSection(nameof(MessageBrokerOptions));
        var conf = section.Get<MessageBrokerOptions>();
        switch (conf.Transport)
        {
            case TransportTypes.SQLSERVER:

                builder.Services.AddSqlServerMigrationHostedService(create: true, delete: false);
                builder.Services.AddOptions<SqlTransportOptions>()
                    .Configure(options =>
                    {
                        options.ConnectionString = conf.ConnectionString;
                        options.Username = conf.Username;
                        options.Password = conf.Password;
                    });
                idServerBuilder.UseMassTransit(o =>
                {
                    o.UsingSqlServer((ctx, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();
                        cfg.ConfigureEndpoints(ctx);
                    });
                });
                break;
        }
    }
}
