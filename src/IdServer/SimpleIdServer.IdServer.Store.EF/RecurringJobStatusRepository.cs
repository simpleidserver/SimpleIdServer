// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class RecurringJobStatusRepository : IRecurringJobStatusRepository
{
    private readonly StoreDbContext _dbContext;

    public RecurringJobStatusRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(RecurringJobStatus recurringJobStatus)
    {
        _dbContext.RecurringJobStatusLst.Add(recurringJobStatus);
    }

    public Task<RecurringJobStatus> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.RecurringJobStatusLst.SingleOrDefaultAsync(r => r.JobId == id, cancellationToken);
    }

    public Task<List<RecurringJobStatus>> Get(List<string> ids, CancellationToken cancellationToken)
    {
        return _dbContext.RecurringJobStatusLst.Where(r => ids.Contains(r.JobId)).ToListAsync(cancellationToken);
    }
}
