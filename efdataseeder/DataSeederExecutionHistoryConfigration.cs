// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfdataSeeder;

public class DataSeederExecutionHistoryConfigration : IEntityTypeConfiguration<DataSeederExecutionHistory>
{
    public void Configure(EntityTypeBuilder<DataSeederExecutionHistory> builder)
    {
        builder.HasKey(h => h.Id);
    }
}
