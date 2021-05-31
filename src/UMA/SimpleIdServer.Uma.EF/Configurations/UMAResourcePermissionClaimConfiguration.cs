// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Uma.Domains;

namespace SimpleIdServer.Uma.EF.Configurations
{
    public class UMAResourcePermissionClaimConfiguration : IEntityTypeConfiguration<UMAResourcePermissionClaim>
    {
        public void Configure(EntityTypeBuilder<UMAResourcePermissionClaim> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
