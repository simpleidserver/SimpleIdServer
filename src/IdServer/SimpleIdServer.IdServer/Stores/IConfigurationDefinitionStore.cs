﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IConfigurationDefinitionStore
{
    void Add(ConfigurationDefinition configurationDefinition);
    Task<List<ConfigurationDefinition>> GetAll(CancellationToken cancellationToken);
}