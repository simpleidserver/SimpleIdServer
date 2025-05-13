// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class MigrationExecutionConfiguration : IEntityTypeConfiguration<MigrationExecution>
{
    public void Configure(EntityTypeBuilder<MigrationExecution> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasMany(m => m.Histories).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
