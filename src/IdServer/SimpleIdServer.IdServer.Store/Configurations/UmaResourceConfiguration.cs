// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UmaResourceConfiguration : IEntityTypeConfiguration<UMAResource>
    {
        public void Configure(EntityTypeBuilder<UMAResource> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(a => a.Scopes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Ignore(r => r.Name);
            builder.Ignore(r => r.Description);
            builder.HasMany(r => r.Translations).WithMany();
            builder.HasMany(r => r.Permissions).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
