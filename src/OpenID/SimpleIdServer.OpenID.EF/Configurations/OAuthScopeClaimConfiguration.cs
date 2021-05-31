// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OAuthScopeClaimConfiguration : IEntityTypeConfiguration<OAuthScopeClaim>
    {
        public void Configure(EntityTypeBuilder<OAuthScopeClaim> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
