// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class GrantConfiguration : IEntityTypeConfiguration<Grant>
    {
        public void Configure(EntityTypeBuilder<Grant> builder)
        {
            builder.HasKey(g => g.Id);
            builder.Property(a => a.Claims).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.HasMany(g => g.Scopes).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
