// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System.Collections.Generic;

namespace SimpleIdServer.Scim;

public class ScimBuilder
{
    public ScimBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    public IServiceCollection Services { get; }

    public ScimBuilder EnableRealm()
    {
        Services.Configure<ScimHostOptions>(o =>
        {
            o.EnableRealm = true;
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