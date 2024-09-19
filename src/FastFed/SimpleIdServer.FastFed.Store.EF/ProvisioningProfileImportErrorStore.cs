// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Store.EF;

public class ProvisioningProfileImportErrorStore : IProvisioningProfileImportErrorStore
{
    private readonly FastFedDbContext _dbContext;

    public ProvisioningProfileImportErrorStore(FastFedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ProvisioningProfileImportError error)
        => _dbContext.ImportErrors.Add(error);

    public void Add(List<ProvisioningProfileImportError> errors)
        => _dbContext.ImportErrors.AddRange(errors);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
