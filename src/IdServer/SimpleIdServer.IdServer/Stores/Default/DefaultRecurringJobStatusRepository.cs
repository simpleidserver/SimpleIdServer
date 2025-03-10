// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultRecurringJobStatusRepository : IRecurringJobStatusRepository
{
    private readonly List<RecurringJobStatus> _recurringJobStatusLst;

    public DefaultRecurringJobStatusRepository(List<RecurringJobStatus> recurringJobStatusLst)
    {
        _recurringJobStatusLst = recurringJobStatusLst;
    }

    public void Add(RecurringJobStatus recurringJobStatus)
    {
        _recurringJobStatusLst.Add(recurringJobStatus);
    }

    public Task<RecurringJobStatus> Get(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_recurringJobStatusLst.SingleOrDefault(r => r.JobId == id));
    }

    public Task<List<RecurringJobStatus>> Get(List<string> ids, CancellationToken cancellationToken)
    {
        var result = _recurringJobStatusLst.Where(r => ids.Contains(r.JobId)).ToList();
        return Task.FromResult(result);
    }
}
