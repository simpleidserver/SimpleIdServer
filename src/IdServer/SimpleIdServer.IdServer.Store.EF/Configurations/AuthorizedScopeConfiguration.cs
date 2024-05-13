// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class AuthorizedScopeConfiguration : IEntityTypeConfiguration<AuthorizedScope>
    {
        public void Configure(EntityTypeBuilder<AuthorizedScope> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(a => a.Resources);
            builder.Ignore(a => a.Audiences);
            builder.HasMany(a => a.AuthorizedResources).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
