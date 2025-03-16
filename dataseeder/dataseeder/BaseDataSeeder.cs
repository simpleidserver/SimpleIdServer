// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace DataSeeder;

public abstract class BaseDataSeeder : IDataSeeder
{
    private readonly IDataSeederExecutionHistoryRepository _dataSeederExecutionHistoryRepository;

    protected BaseDataSeeder(IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository)
    {
        _dataSeederExecutionHistoryRepository = dataSeederExecutionHistoryRepository;
    }

    public abstract bool IsBeforeDeployment { get; }

    public abstract string Name { get; }

    public async Task Apply(CancellationToken cancellationToken)
    {
        var executionHistory = await _dataSeederExecutionHistoryRepository.Get(Name, cancellationToken);
        if (executionHistory == null)
        {
            return;
        }

        await Execute(cancellationToken);
        _dataSeederExecutionHistoryRepository.Add(new DataSeederExecutionHistory
        {
            Id = Guid.NewGuid().ToString(),
            Name = Name,
            ExecutionDateTime = DateTime.UtcNow
        });
        await _dataSeederExecutionHistoryRepository.SaveChanges(cancellationToken);
    }

    protected abstract Task Execute(CancellationToken cancellationToken);
}
