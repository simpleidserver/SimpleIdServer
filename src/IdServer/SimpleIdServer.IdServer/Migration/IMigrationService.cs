// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migration;

public interface IMigrationService
{
    string Name
    {
        get;
    }

    Task<List<Scope>> ExtractApiScopes(ExtractParameter parameter, CancellationToken cancellationToken);

    Task<List<Scope>> ExtractIdentityScopes(ExtractParameter parameter, CancellationToken cancellationToken);
    
    Task<List<ApiResource>> ExtractApiResources(ExtractParameter parameter, CancellationToken cancellationToken);

    Task<List<Client>> ExtractClients(ExtractParameter parameter, CancellationToken cancellationToken);

    Task<int> NbGroups(CancellationToken cancellationToken);

    Task<List<Group>> ExtractGroups(ExtractParameter parameter, CancellationToken cancellationToken);

    Task<List<User>> ExtractUsers(ExtractParameter parameter, CancellationToken cancellationToken);
}
