// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IUmaResourceRepository
{
    IQueryable<UMAResource> Query();
    void Add(UMAResource resource);
    void Delete(UMAResource resource);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
