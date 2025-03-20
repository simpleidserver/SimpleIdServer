// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class DistributedCacheConfiguration : IEntityTypeConfiguration<DistributedCache>
{
    public void Configure(EntityTypeBuilder<DistributedCache> builder)
    {
        builder.ToTable("DistributedCache");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasMaxLength(449);
    }
}
