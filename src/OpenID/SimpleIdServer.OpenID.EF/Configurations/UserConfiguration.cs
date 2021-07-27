// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Common.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Ignore(u => u.Claims);
            builder.HasMany(u => u.Sessions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.OAuthUserClaims).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.ExternalAuthProviders).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}