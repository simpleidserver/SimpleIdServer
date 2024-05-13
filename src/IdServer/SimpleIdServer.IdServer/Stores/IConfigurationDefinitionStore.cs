// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;

namespace SimpleIdServer.IdServer.Stores;

public interface IConfigurationDefinitionStore
{
    IQueryable<ConfigurationDefinition> Query();
}
