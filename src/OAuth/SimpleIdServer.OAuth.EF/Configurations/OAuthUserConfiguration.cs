// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.EF.Configurations
{
    public class OAuthUserConfiguration : IEntityTypeConfiguration<OAuthUser>
    {
        public void Configure(EntityTypeBuilder<OAuthUser> builder)
        {
            builder.HasBaseType<User>();
            builder.HasMany(u => u.Consents).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}