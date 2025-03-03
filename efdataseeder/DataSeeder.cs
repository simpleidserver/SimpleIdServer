﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
namespace EfdataSeeder;

public abstract class DataSeeder<T, U> : IDataSeeder where T : DataSeederDbContext<U> where U : DbContext
{
    public DataSeeder(T dbContext)
    {
        DbContext = dbContext;
    }

    protected T DbContext { get; private set; }

    protected abstract string Name { get; }

    public async Task Apply(CancellationToken cancellationToken)
    {
        var existingExecutionHistory = await DbContext.ExecutionHistories.SingleOrDefaultAsync(e => e.Name == Name, cancellationToken);
        if (existingExecutionHistory != null)
        {
            return;
        }

        await Up(cancellationToken);
        DbContext.ExecutionHistories.Add(new DataSeederExecutionHistory
        {
            Id = Guid.NewGuid().ToString(),
            Name = Name,
            ExecutionDateTime = DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected abstract Task Up(CancellationToken cancellationTokenn);
}
