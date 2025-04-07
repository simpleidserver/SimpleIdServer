// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim;

public class ScimBuilder
{
    public ScimBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public IServiceCollection ServiceCollection { get; }

    public ScimBuilder EnableApiKeyAuthentication(ApiKeysConfiguration configuration)
    {
        ServiceCollection.AddSingleton(configuration);
        ServiceCollection.AddAuthentication("ApiKeys")
            .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
            {
                options.Realm = "Sample Web API";
                options.KeyName = "Authorization";
            });
        ServiceCollection.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
        return this;
    }

    public ScimBuilder EnableRealm()
    {
        ServiceCollection.Configure<ScimHostOptions>(o =>
        {
            o.EnableRealm = true;
        });
        return this;
    }

    public ScimBuilder PublishLargeMessage()
    {
        ServiceCollection.Configure<ScimHostOptions>(o =>
        {
            o.IsBigMessagePublished = true;
        });
        return this;
    }

    public ScimBuilder EnableMasstransit(Action<IBusRegistrationConfigurator> cb)
    {
        if (ServiceCollection.Any(s => s.ServiceType == typeof(IBus)))
        {
            throw new InvalidOperationException("MassTransit is already configured by the AddScim operation. To disable this configuration, set the skipMassTransitRegistration parameter to true.");
        }

        ServiceCollection.AddMassTransit(x =>
        {
            cb(x);
        });
        return this;
    }

    public ScimBuilder AddSchemas(List<SCIMSchema> schemas)
    {
        ServiceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
        ServiceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
        return this;
    }

    public ScimBuilder ImportSchemas(Dictionary<string, string> dic)
    {
        var schemaLst = new List<SCIMSchema>();
        foreach (var kvp in dic)
        {
            schemaLst.Add(SCIMSchemaExtractor.Extract(kvp.Value, kvp.Key));
        }

        ServiceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemaLst));
        ServiceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemaLst));
        return this;
    }

    public ScimBuilder AddAttributeMapping(List<SCIMAttributeMapping> attributeMappingLst)
    {
        ServiceCollection.AddSingleton<ISCIMAttributeMappingQueryRepository>(new DefaultAttributeMappingQueryRepository(attributeMappingLst));
        return this;
    }
}
