// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using Microsoft.EntityFrameworkCore;

namespace SimpleIdServer.IdServer.Store.EF;

public class DataSeederExecutionHistoryRepository : IDataSeederExecutionHistoryRepository
{
    private readonly StoreDbContext _dbContext;

    public DataSeederExecutionHistoryRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(DataSeederExecutionHistory dataSeederExecutionHistory)
    {
        _dbContext.ExecutionHistories.Add(dataSeederExecutionHistory);
    }

    public Task<DataSeederExecutionHistory> Get(string name, CancellationToken cancellationToken)
    {
        return _dbContext.ExecutionHistories.SingleOrDefaultAsync(h => h.Name == name, cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
