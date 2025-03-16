// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace EfdataSeeder;

public abstract class DataSeederDbContext<T> : DbContext where T : DbContext
{
    public DataSeederDbContext(DbContextOptions<T> options) : base(options)
    {
        
    }

    public DbSet<DataSeederExecutionHistory> ExecutionHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DataSeederExecutionHistoryConfigration());
        Update(modelBuilder);
    }

    protected abstract void Update(ModelBuilder modelBuilder);
}
