// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Migrate;

public class MigrationResult
{
    public MigrationResult(List<Client> clients, List<Scope> scopes, List<ApiResource> apiResources)
    {
        Clients = clients;
        Scopes = scopes;
        ApiResources = apiResources;
    }

    public List<Client> Clients
    {
        get; private set;
    }

    public List<Scope> Scopes
    {
        get; private set;
    }

    public List<ApiResource> ApiResources
    {
        get; private set;
    }
}
