// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF.Extensions;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UMAResourcePermissionConfiguration : IEntityTypeConfiguration<UMAResourcePermission>
    {
        public void Configure(EntityTypeBuilder<UMAResourcePermission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(a => a.Scopes).ConfigureSerializer();
            builder.HasMany(p => p.Claims).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
