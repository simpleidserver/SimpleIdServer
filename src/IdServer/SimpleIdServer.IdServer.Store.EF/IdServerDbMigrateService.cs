// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.EntityFrameworkCore;

namespace SimpleIdServer.IdServer.Store.EF;

public class IdServerDbMigrateService : IDbMigrateService
{
    private readonly StoreDbContext _dbContext;

    public IdServerDbMigrateService(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Migrate(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.IsInMemory())
        {
            return Task.CompletedTask;
        }

        return _dbContext.Database.MigrateAsync(cancellationToken);
    }
}
