// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IAuthenticationSchemeProviderDefinitionRepository
{
    Task<AuthenticationSchemeProviderDefinition> Get(string name, CancellationToken cancellationToken);
    Task<List<AuthenticationSchemeProviderDefinition>> GetAll(CancellationToken cancellationToken);
    void Add(AuthenticationSchemeProviderDefinition definition);
}
