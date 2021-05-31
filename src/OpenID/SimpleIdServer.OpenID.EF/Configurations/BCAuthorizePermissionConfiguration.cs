// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class BCAuthorizePermissionConfiguration : IEntityTypeConfiguration<BCAuthorizePermission>
    {
        public void Configure(EntityTypeBuilder<BCAuthorizePermission> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
