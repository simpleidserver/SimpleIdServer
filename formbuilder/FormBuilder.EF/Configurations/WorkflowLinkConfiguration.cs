// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace FormBuilder.EF.Configurations;

public class WorkflowLinkConfiguration : IEntityTypeConfiguration<WorkflowLink>
{
    public void Configure(EntityTypeBuilder<WorkflowLink> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Ignore(w => w.IsLinkHoverStep);
        builder.Ignore(w => w.IsHover);
        builder.Property(w => w.Source).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<WorkflowLinkSource>(v, (JsonSerializerOptions)null));
        builder.Property(w => w.Targets).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<List<WorkflowLinkTarget>>(v, (JsonSerializerOptions)null));
    }
}
