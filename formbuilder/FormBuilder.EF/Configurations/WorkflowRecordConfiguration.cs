// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.EF.Configurations;

public class WorkflowRecordConfiguration : IEntityTypeConfiguration<WorkflowRecord>
{
    public void Configure(EntityTypeBuilder<WorkflowRecord> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasMany(c => c.Steps).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Links).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
