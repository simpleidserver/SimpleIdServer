// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Azure.Storage.Blobs;
using MassTransit;
using MassTransit.MessageData;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence.MongoDB;
using SimpleIdServer.Scim.Startup.Configurations;
using SimpleIdServer.Scim.Startup.Consumers;
using System;
using System.IO;

namespace SimpleIdServer.Scim.Startup.Infrastructures;

public class ScimServerSetup
{
    public static void ConfigureScim(WebApplicationBuilder webApplicationBuilder, StorageConfiguration storageConfiguration, ApiKeysConfiguration apiKeysConfiguration)
    {
        var massTransitConf = webApplicationBuilder.Configuration.Get<MassTransitStorageConfiguration>();
        var builder = webApplicationBuilder.Services.AddScim(o =>
            {
                o.IgnoreUnsupportedCanonicalValues = false;
            })
            .UpdateApiKeys(apiKeysConfiguration)
            .EnableSwagger()
            .ConfigureMassTransit(cb => ConfigureMessageBroker(webApplicationBuilder, cb, massTransitConf));
        if(storageConfiguration.Type == StorageTypes.MONGODB)
        {
            builder.UseMongodbStorage(d =>
            {
                ConfigureMongodbStorage(storageConfiguration, d);
            });
        }
        else
        {
            builder.UseEfStore((db) =>
            {
                ConfigureEfStorage(storageConfiguration, db);
            }, (o) =>
            {
                o.DefaultSchema = "scim";
                o.IgnoreBulkOperation = storageConfiguration.Type == StorageTypes.SQLITE;
            });
        }

        var isRealmEnabled = bool.Parse(webApplicationBuilder.Configuration["IsRealmEnabled"]);
        if (isRealmEnabled)
        {
            builder.EnableRealm();
        }

        if (massTransitConf.IsEnabled)
        {
            builder.PublishLargeMessage();
        }
    }

    private static void ConfigureMessageBroker(WebApplicationBuilder builder, IBusRegistrationConfigurator configurator, MassTransitStorageConfiguration massTransitConf)
    {
        var repository = ConfigureMessageBrokerRepository(builder.Services, massTransitConf);
        if (!massTransitConf.IsEnabled)
        {
            configurator.AddConsumer<IntegrationEventConsumer>();
        }
        else
        {
            configurator.AddConsumer<BigMessageConsumer>();
        }

        configurator.UsingInMemory((context, cfg) =>
        {
            cfg.UseMessageData(repository);
            cfg.ConfigureEndpoints(context);
        });
    }

    private static void ConfigureEfStorage(StorageConfiguration conf, DbContextOptionsBuilder dbContextOptsCallback = null)
    {
        switch (conf.Type)
        {
            case StorageTypes.SQLSERVER:
                dbContextOptsCallback.UseSqlServer(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SimpleIdServer.Scim.SqlServerMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.POSTGRE:
                dbContextOptsCallback.UseNpgsql(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SimpleIdServer.Scim.PostgreMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.MYSQL:
                dbContextOptsCallback.UseMySql(conf.ConnectionString, ServerVersion.AutoDetect(conf.ConnectionString), o =>
                {
                    o.MigrationsAssembly("SimpleIdServer.Scim.MySQLMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
                });
                break;
            case StorageTypes.SQLITE:
                dbContextOptsCallback.UseSqlite(conf.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SimpleIdServer.Scim.SqliteMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.INMEMORY:
                dbContextOptsCallback.UseInMemoryDatabase(conf.ConnectionString);
                break;
        }
    }

    private static void ConfigureMongodbStorage(StorageConfiguration conf, MongoDbOptions mongoDbOptions)
    {
        mongoDbOptions.ConnectionString = conf.ConnectionString;
        mongoDbOptions.Database = "scim";
        mongoDbOptions.CollectionMappings = "mappings";
        mongoDbOptions.CollectionRepresentations = "representations";
        mongoDbOptions.CollectionSchemas = "schemas";
        mongoDbOptions.SupportTransaction = false;
    }

    private static IMessageDataRepository ConfigureMessageBrokerRepository(IServiceCollection services, MassTransitStorageConfiguration conf)
    {
        IMessageDataRepository repository = null;
        if (!conf.IsEnabled)
        {
            repository = new InMemoryMessageDataRepository();
            services.AddSingleton(repository);
            return repository;
        }

        switch (conf.Type)
        {
            case MassTransitStorageTypes.INMEMORY:
                repository = new InMemoryMessageDataRepository();
                break;
            case MassTransitStorageTypes.DIRECTORY:
                repository = new FileSystemMessageDataRepository(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
                break;
            case MassTransitStorageTypes.AZURESTORAGE:
                var client = new BlobServiceClient(conf.ConnectionString);
                repository = client.CreateMessageDataRepository("message-data");
                break;
        }

        services.AddSingleton(repository);
        return repository;
    }
}
