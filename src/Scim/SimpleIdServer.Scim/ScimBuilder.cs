// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        Services = serviceCollection;
    }

    public IServiceCollection Services { get; }

    public ScimBuilder UpdateApiKeys(ApiKeysConfiguration configuration)
    {
        var type = Services.Single(c => c.ServiceType == typeof(ApiKeysConfiguration));
        Services.Remove(type);
        Services.AddSingleton(configuration);
        return this;
    }

    public ScimBuilder EnableRealm()
    {
        Services.Configure<ScimHostOptions>(o =>
        {
            o.EnableRealm = true;
        });
        return this;
    }

    public ScimBuilder PublishLargeMessage()
    {
        Services.Configure<ScimHostOptions>(o =>
        {
            o.IsBigMessagePublished = true;
        });
        return this;
    }

    public ScimBuilder ConfigureMassTransit(Action<IBusRegistrationConfigurator> cb)
    {
        if (Services.Any(s => s.ServiceType == typeof(IBus)))
        {
            throw new InvalidOperationException("MassTransit is already configured by the AddScim operation. To disable this configuration, set the skipMassTransitRegistration parameter to true.");
        }

        Services.AddMassTransit(x =>
        {
            cb(x);
        });
        return this;
    }

    public ScimBuilder AddInMemorySchemas(List<SCIMSchema> schemas)
    {
        Services.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
        Services.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
        return this;
    }

    public ScimBuilder AddInMemoryAttributeMappings(List<SCIMAttributeMapping> attributeMappingLst)
    {
        Services.AddSingleton<ISCIMAttributeMappingQueryRepository>(new DefaultAttributeMappingQueryRepository(attributeMappingLst));
        return this;
    }

    public ScimBuilder AddInMemoryRealms(List<Realm> realms)
    {
        Services.AddSingleton<IRealmRepository>(new DefaultRealmRepository(realms));
        return this;
    }
}