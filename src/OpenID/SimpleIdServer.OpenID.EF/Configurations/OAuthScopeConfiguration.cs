// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OAuthScopeConfiguration : IEntityTypeConfiguration<OAuthScope>
    {
        public void Configure(EntityTypeBuilder<OAuthScope> builder)
        {
            builder.HasKey(s => s.Name);
            builder.HasIndex(s => s.Name).IsUnique();
            builder.HasMany(s => s.Claims).WithOne();
            builder.Ignore(s => s.Clients);
        }
    }
}
