// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class MigrationExecutionHistoryConfiguration : IEntityTypeConfiguration<MigrationExecutionHistory>
{
    public void Configure(EntityTypeBuilder<MigrationExecutionHistory> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(a => a.Errors).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
    }
}
