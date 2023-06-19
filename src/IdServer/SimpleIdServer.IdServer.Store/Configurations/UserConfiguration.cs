// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Ignore(u => u.Claims);
            builder.HasMany(u => u.Sessions).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.OAuthUserClaims).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Credentials).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.ExternalAuthProviders).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Consents).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Devices).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.CredentialOffers).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Realms).WithOne(u => u.User).HasForeignKey(u => u.UsersId);
        }
    }
}
