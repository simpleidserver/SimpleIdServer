// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Uma.Domains;
using System;
using System.Linq;

namespace SimpleIdServer.Uma.EF.Configurations
{
    public class UMAResourcePermissionConfiguration : IEntityTypeConfiguration<UMAResourcePermission>
    {
        public void Configure(EntityTypeBuilder<UMAResourcePermission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Scopes)
                .HasConversion(
                p => string.Join(',', p),
                p => p.Split(',', StringSplitOptions.None).ToList());
            builder.HasMany(p => p.Claims).WithOne();
        }
    }
}
