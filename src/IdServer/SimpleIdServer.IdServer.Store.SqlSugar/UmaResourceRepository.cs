// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

internal class UmaResourceRepository : IUmaResourceRepository
{
    public void Add(UMAResource resource)
    {
        throw new NotImplementedException();
    }

    public void Delete(UMAResource resource)
    {
        throw new NotImplementedException();
    }

    public Task<UMAResource> Get(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<UMAResource>> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<UMAResource>> GetByIds(List<string> resourceIds, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
