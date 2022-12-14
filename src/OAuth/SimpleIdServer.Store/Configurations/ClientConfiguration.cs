// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Domains;

namespace SimpleIdServer.Store.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.ClientId);
            builder.Property(a => a.GrantTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None));
            builder.Property(a => a.ResponseTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None));
            builder.Property(a => a.RedirectionUrls).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None));
            builder.Property(a => a.PostLogoutRedirectUris).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None));
            builder.Property(a => a.Contacts).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None));
            builder.HasMany(c => c.Translations).WithMany();
            builder.HasMany(c => c.Scopes).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.SerializedJsonWebKeys).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
