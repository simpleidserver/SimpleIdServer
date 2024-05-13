// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IUmaResourceRepository
{
    Task<UMAResource> Get(string id, CancellationToken cancellationToken);
    Task<List<UMAResource>> GetByIds(List<string> resourceIds, CancellationToken cancellationToken);
    Task<List<UMAResource>> GetAll(CancellationToken cancellationToken);
    void Add(UMAResource resource);
    void Delete(UMAResource resource);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
