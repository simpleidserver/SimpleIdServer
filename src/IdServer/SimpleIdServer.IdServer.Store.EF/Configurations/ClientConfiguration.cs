// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(a => a.ClientRegistrationTypesSupported).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.GrantTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.ResponseTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.RedirectionUrls).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.PostLogoutRedirectUris).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.Contacts).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.DefaultAcrValues).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.AuthorizationDataTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.SubjectSyntaxTypesSupported).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.HasMany(c => c.Translations).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.SerializedJsonWebKeys).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.DeviceAuthCodes).WithOne(a => a.Client).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Secrets).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Scopes).WithMany(s => s.Clients);
            builder.HasMany(c => c.Realms).WithMany(s => s.Clients);
            builder.Ignore(c => c.JsonWebKeys);
            builder.Ignore(c => c.Parameters);
            builder.Ignore(c => c.Scope);
        }
    }
}
