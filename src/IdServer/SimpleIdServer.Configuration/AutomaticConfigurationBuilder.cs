// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Configuration;

public class AutomaticConfigurationOptions
{
    internal List<AutomaticConfigurationRecord> ConfigurationDefinitions = new List<AutomaticConfigurationRecord>();
    internal IKeyValueConnector? KeyValueConnector = null;

    public AutomaticConfigurationOptions UseEFConnector(Action<DbContextOptionsBuilder>? action = null)
    {
        var builder = new DbContextOptionsBuilder<StoreDbContext>();
        if (action == null) builder.UseInMemoryDatabase("Configuration");
        else action(builder);
        var dbContext = new StoreDbContext(builder.Options);
        KeyValueConnector = new EFKeyValueConnector(dbContext);
        return this;
    }

    public AutomaticConfigurationOptions UseRedisConnector()
    {
        return this;
    }

    public AutomaticConfigurationOptions Add<T>()
    {
        var type = typeof(T);
        ConfigurationDefinitions.Add(new AutomaticConfigurationRecord(type, type.Name, ConfigurationDefinitionExtractor.Extract<T>()));
        return this;
    }
}

public class AutomaticConfigurationRecord
{
    public AutomaticConfigurationRecord(Type type, string name, ConfigurationDefinition definition)
    {
        Type = type;
        Name = name;
        Definition = definition;
    }

    public Type Type { get; private set; }
    public string Name { get; private set; }
    public ConfigurationDefinition Definition { get; private set; }
}
