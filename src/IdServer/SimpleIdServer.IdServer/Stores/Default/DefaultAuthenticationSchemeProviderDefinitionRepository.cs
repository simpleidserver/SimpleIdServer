// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultAuthenticationSchemeProviderDefinitionRepository : IAuthenticationSchemeProviderDefinitionRepository
{
    private readonly List<AuthenticationSchemeProviderDefinition> _definitions;

    public DefaultAuthenticationSchemeProviderDefinitionRepository(List<AuthenticationSchemeProviderDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void Add(AuthenticationSchemeProviderDefinition definition)
    {
        _definitions.Add(definition);
    }

    public Task<AuthenticationSchemeProviderDefinition> Get(string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_definitions.SingleOrDefault(a => a.Name == name));
    }

    public Task<List<AuthenticationSchemeProviderDefinition>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_definitions);
    }
}
