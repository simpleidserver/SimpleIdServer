// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class DataSeederExecutionHistoryConfiguration : IEntityTypeConfiguration<DataSeederExecutionHistory>
{
    public void Configure(EntityTypeBuilder<DataSeederExecutionHistory> builder)
    {
        builder.HasKey(d => d.Id);
    }
}
