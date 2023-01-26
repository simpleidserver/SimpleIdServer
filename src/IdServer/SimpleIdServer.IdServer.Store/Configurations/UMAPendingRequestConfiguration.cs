// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UMAPendingRequestConfiguration : IEntityTypeConfiguration<UMAPendingRequest>
    {
        public void Configure(EntityTypeBuilder<UMAPendingRequest> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.Scopes)
                .HasConversion(p => string.Join(',', p),
                p => p.Split(',', StringSplitOptions.None));
            builder.HasOne(p => p.Resource).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
