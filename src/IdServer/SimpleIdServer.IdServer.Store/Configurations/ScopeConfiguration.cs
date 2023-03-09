// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class ScopeConfiguration : IEntityTypeConfiguration<Scope>
    {
        public void Configure(EntityTypeBuilder<Scope> builder)
        {
            builder.HasKey(s => s.Name);
            builder.HasIndex(s => s.Name).IsUnique();
            builder.HasMany(s => s.ClaimMappers).WithOne(s => s.Scope).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.Realms).WithMany(s => s.Scopes);
        }
    }
}