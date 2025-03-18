﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Community.Microsoft.Extensions.Caching.PostgreSql;
using EfdataSeeder;
using Hangfire;
using Hangfire.MySql;
using Hangfire.PostgreSql;
using Hangfire.SQLite;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite.AspNetCore;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;
using SimpleIdServer.IdServer.Startup.Conf.Migrations.BeforeDeployment;
using SimpleIdServer.IdServer.Startup.Configurations;
using SimpleIdServer.IdServer.TokenTypes;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace SimpleIdServer.IdServer.Startup.Conf;

public class SidServerSetup
{
    public static void ConfigureIdServer(WebApplicationBuilder webApplicationBuilder, IdentityServerConfiguration configuration)
    {
        var section = webApplicationBuilder.Configuration.GetSection(nameof(ScimClientOptions));
        var conf = section.Get<ScimClientOptions>();
        var idServerBuilder = webApplicationBuilder.AddSidIdentityServer(c =>
            {
                c.ForceHttps = configuration.ForceHttps;
                c.Authority = configuration.Authority;
                c.ScimClientOptions = conf;
                if (!string.IsNullOrWhiteSpace(configuration.SessionCookieNamePrefix))
                {
                    c.SessionCookieName = configuration.SessionCookieNamePrefix;
                }
            })
            .IgnoreCertificateError()
            .EnableFapiSecurityProfile()
            .AddPwdAuthentication()
            .AddEmailAuthentication()
            .AddOtpAuthentication()
            .AddSmsAuthentication()
            .AddMobileAuthentication(o =>
            {
                // TODO : Simplify ???
                var authority = configuration.Authority;
                var url = new Uri(authority);
                o.ServerName = "SimpleIdServer";
                o.ServerDomain = url.Host;
                o.Origins = new HashSet<string> { authority };
            })
            .AddWebauthnAuthentication()
            .AddVpAuthentication()
            .AddSamlIdp()
            .AddWsFederation()
            .AddOpenidFederation(o =>
            {
                o.IsFederationEnabled = true;
            })
            .AddScimProvisioning()
            .AddLdapProvisioning()
            .AddFcmNotification()
            .AddGotifyNotification()
            .AddSwagger(opt =>
            {
                opt.IncludeDocumentation<AccessTokenTypeService>();
            })
            .ConfigureHangfire((c) =>
            {
                ConfigureHangfire(webApplicationBuilder, c);
            })
            // TODO : Add redis !!
            .ConfigureKeyValueStore((c) =>
            {
            })
            .ConfigureMasstransit(o =>
            {
                ConfigureMessageBroker(webApplicationBuilder, o);
            })
            .ConfigureAuthCookie(o =>
            {
                if (!string.IsNullOrWhiteSpace(configuration.AuthCookieNamePrefix))
                {
                    o.Cookie.Name = configuration.AuthCookieNamePrefix;
                }
            })
            .UseEfStore((c) =>
            {
                ConfigureIdserverStorage(webApplicationBuilder, c);
            }, (c) =>
            {
                ConfigureFormbuilderStorage(webApplicationBuilder, c);
            });
        if (configuration.IsForwardedEnabled)
        {
            idServerBuilder.ForwardHttpHeader();
        }

        if (configuration.IsMtlsAuthenticationEnabled)
        {
            idServerBuilder.EnableMtlsAuthentication(configuration.ClientCertificateMode ?? ClientCertificateMode.AllowCertificate, callback: cb =>
            {
                cb.AllowedCertificateTypes = CertificateTypes.All;
                cb.RevocationMode = X509RevocationMode.NoCheck;
            });
        }

        if (configuration.IsRealmEnabled)
        {
            idServerBuilder.EnableRealm();
        }

        ConfigureDistributedCache(webApplicationBuilder);
    }

    private static void ConfigureHangfire(WebApplicationBuilder webApplicationBuilder, IGlobalConfiguration configuration)
    {
        var section = webApplicationBuilder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        switch (conf.Type)
        {
            case StorageTypes.INMEMORY:
                configuration.UseInMemoryStorage();
                break;
            case StorageTypes.SQLSERVER:
                configuration.UseSqlServerStorage(conf.ConnectionString);
                break;
            case StorageTypes.POSTGRE:
                configuration.UsePostgreSqlStorage(conf.ConnectionString);
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
                configuration.UseSQLiteStorage(conf.ConnectionString);
                break;
        }
    }

    private static void ConfigureIdserverStorage(WebApplicationBuilder builder, DbContextOptionsBuilder b)
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
                b.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
                break;
        }
    }

    private static void ConfigureFormbuilderStorage(WebApplicationBuilder builder, DbContextOptionsBuilder b)
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
                builder.Services.AddSqliteCache(options =>
                {
                    options.CachePath = conf.ConnectionString;
                });
                break;
        }
    }

    private static void ConfigureMessageBroker(WebApplicationBuilder builder, IBusRegistrationConfigurator configurator)
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
                configurator.UsingSqlServer((ctx, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();
                    cfg.ConfigureEndpoints(ctx);
                });
                break;
        }
    }

    public static void ConfigureDataseeder(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDataSeeder, FormAndWorkflowDataSeeder>();
        builder.Services.AddTransient<IDataSeeder, ClientTypeDataSeeder>();
    }
}
