// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class UmaPendingRequestRepository : IUmaPendingRequestRepository
{
    public void Add(UMAPendingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<List<UMAPendingRequest>> GetByPermissionTicketId(string permissionTicketId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<UMAPendingRequest>> GetByUsername(string realm, string userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<UMAPendingRequest> Query()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
