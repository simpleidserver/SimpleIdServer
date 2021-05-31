// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;
using System;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class BCAuthorizeConfiguration : IEntityTypeConfiguration<BCAuthorize>
    {
        public void Configure(EntityTypeBuilder<BCAuthorize> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Scopes).HasConversion(
                s => string.Join(',', s),
                s => s.Split(',', StringSplitOptions.None));
            builder.HasMany(b => b.Permissions).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
