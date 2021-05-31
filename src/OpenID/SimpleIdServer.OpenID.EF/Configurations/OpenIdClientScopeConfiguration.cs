// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OpenIdClientScopeConfiguration : IEntityTypeConfiguration<OpenIdClientScope>
    {
        public void Configure(EntityTypeBuilder<OpenIdClientScope> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.HasOne(c => c.Scope).WithMany();
        }
    }
}
