// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace DataSeeder;

public interface IDataSeederExecutionHistoryRepository
{
    Task<DataSeederExecutionHistory> Get(string name, CancellationToken cancellationToken);
    void Add(DataSeederExecutionHistory dataSeederExecutionHistory);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class DefaultDataSeederExecutionHistoryRepository : IDataSeederExecutionHistoryRepository
{
    private readonly List<DataSeederExecutionHistory> _executionHistories;

    public DefaultDataSeederExecutionHistoryRepository()
    {
        _executionHistories = new List<DataSeederExecutionHistory>();
    }

    public Task<DataSeederExecutionHistory> Get(string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_executionHistories.SingleOrDefault(h => h.Name == name));
    }

    public void Add(DataSeederExecutionHistory dataSeederExecutionHistory)
    {
        _executionHistories.Add(dataSeederExecutionHistory);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}
