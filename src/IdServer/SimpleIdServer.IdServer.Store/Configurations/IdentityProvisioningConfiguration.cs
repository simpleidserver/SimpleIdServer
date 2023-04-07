// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class IdentityProvisioningConfiguration : IEntityTypeConfiguration<IdentityProvisioning>
    {
        public void Configure(EntityTypeBuilder<IdentityProvisioning> builder)
        {
            builder.HasKey(i => i.Id);
            builder.HasMany(g => g.Properties).WithOne(g => g.IdentityProvisioning).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(i => i.Histories).WithOne(g => g.IdentityProvisioning).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(i => i.Users).WithOne(g => g.IdentityProvisioning).HasForeignKey(g => g.IdentityProvisioningId);
            builder.HasMany(i => i.Realms).WithMany(g => g.IdentityProvisioningLst);
        }
    }
}
