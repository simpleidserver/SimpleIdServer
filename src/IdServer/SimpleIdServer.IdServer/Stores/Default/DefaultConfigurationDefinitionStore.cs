// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultConfigurationDefinitionStore : IConfigurationDefinitionStore
{
    private readonly List<ConfigurationDefinition> _configurationDefinitions = new List<ConfigurationDefinition>();

    public DefaultConfigurationDefinitionStore()
    {
        
    }

    public void Add(ConfigurationDefinition configurationDefinition)
    {
        _configurationDefinitions.Add(configurationDefinition);
    }

    public Task<ConfigurationDefinition> Get(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_configurationDefinitions.SingleOrDefault(c => c.Id == id));
    }

    public Task<List<ConfigurationDefinition>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_configurationDefinitions);
    }
}
