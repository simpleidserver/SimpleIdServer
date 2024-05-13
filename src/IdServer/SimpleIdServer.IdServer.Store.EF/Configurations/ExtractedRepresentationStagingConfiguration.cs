// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class ExtractedRepresentationStagingConfiguration : IEntityTypeConfiguration<ExtractedRepresentationStaging>
{
    public void Configure(EntityTypeBuilder<ExtractedRepresentationStaging> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(a => a.GroupIds).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
    }
}
