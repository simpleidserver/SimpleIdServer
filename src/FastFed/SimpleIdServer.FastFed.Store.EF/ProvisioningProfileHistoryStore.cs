// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.Models;

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Store.EF;

public class ProvisioningProfileHistoryStore : IProvisioningProfileHistoryStore
{
    private readonly FastFedDbContext _dbContext;

    public ProvisioningProfileHistoryStore(FastFedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ProvisioningProfileHistory record)
        => _dbContext.ProvisioningProfileHistories.Add(record);

    public Task<ProvisioningProfileHistory> Get(string profileName, CancellationToken cancellationToken)
        => _dbContext.ProvisioningProfileHistories.SingleOrDefaultAsync(p => p.ProfileName == profileName, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
