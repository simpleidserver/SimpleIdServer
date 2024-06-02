// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class UmaPendingRequestRepository : IUmaPendingRequestRepository
{
    private readonly StoreDbContext _dbContext;

    public UmaPendingRequestRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<UMAPendingRequest>> GetByPermissionTicketId(string permissionTicketId, CancellationToken cancellationToken)
    {
        return _dbContext.UmaPendingRequest.Where(r => r.TicketId == permissionTicketId).ToListAsync(cancellationToken);
    }

    public Task<List<UMAPendingRequest>> GetByUsername(string realm, string userName, CancellationToken cancellationToken)
    {
        return _dbContext.UmaPendingRequest
            .Include(p => p.Resource).ThenInclude(p => p.Translations)
            .Where(r => (r.Owner == userName || r.Requester == userName) && r.Realm == realm)
            .ToListAsync(cancellationToken);
    }

    public void Add(UMAPendingRequest request) => _dbContext.UmaPendingRequest.Add(request);

    public void Update(UMAPendingRequest request)
    {

    }
}
