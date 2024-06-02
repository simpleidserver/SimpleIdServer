// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Configuration;

public class AutomaticConfigurationOptions
{
    internal List<AutomaticConfigurationRecord> ConfigurationDefinitions = new List<AutomaticConfigurationRecord>();

    public AutomaticConfigurationOptions(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; set; }

    public AutomaticConfigurationOptions UseEFConnector()
    {
        Services.AddTransient<IKeyValueConnector, EFKeyValueConnector>();
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
