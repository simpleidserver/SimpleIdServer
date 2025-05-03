// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdServerLogging.Conf.Migrations.BeforeDeployment;

public class DropDistributedCacheDataSeeder : BaseBeforeDeploymentDataSeeder
{
    private List<string> _migrationNames = new List<string>
    {
        "20250325142013_ConfigureDistributedCache",
        "20250325144710_ConfigureDistributedCache",
        "20250325144053_ConfigureDistributedCache",
        "20250325143903_ConfigureDistributedCache"
    };
    private readonly StoreDbContext _dbContext;

    public DropDistributedCacheDataSeeder(StoreDbContext dbContext, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _dbContext = dbContext;
    }

    public override string Name => nameof(DropDistributedCacheDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        if(_dbContext.Database.IsInMemory())
        {
            return;
        }

        var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);
        if(!appliedMigrations.Any(m => _migrationNames.Contains(m)))
        {
            _dbContext.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS DistributedCache");
        }
    }
}
