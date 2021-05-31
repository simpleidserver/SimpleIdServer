// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Uma.Domains;
using System;

namespace SimpleIdServer.Uma.EF.Configurations
{
    public class UMAResourceConfiguration : IEntityTypeConfiguration<UMAResource>
    {
        public void Configure(EntityTypeBuilder<UMAResource> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Scopes).HasConversion(
                r => string.Join(',', r),
                r => r.Split(',', StringSplitOptions.None));
            builder.Ignore(r => r.Descriptions);
            builder.Ignore(r => r.Names);
            builder.HasMany(r => r.Translations).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.Permissions).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
