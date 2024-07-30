// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class RealmRoleConfiguration : IEntityTypeConfiguration<RealmRole>
{
    public void Configure(EntityTypeBuilder<RealmRole> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasMany(r => r.Scopes).WithOne(p => p.Role).HasForeignKey(p => p.RealmRoleId);
    }
}
