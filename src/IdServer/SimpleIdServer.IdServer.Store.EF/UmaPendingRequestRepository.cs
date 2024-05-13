// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

    public IQueryable<UMAPendingRequest> Query() => _dbContext.UmaPendingRequest;

    public void Add(UMAPendingRequest request) => _dbContext.UmaPendingRequest.Add(request);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
