// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OpenIdClientConfiguration : IEntityTypeConfiguration<OpenIdClient>
    {
        public void Configure(EntityTypeBuilder<OpenIdClient> builder)
        {
            builder.HasKey(c => c.ClientId);
            builder.Property(a => a.GrantTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.Property(a => a.ResponseTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.Property(a => a.RedirectionUrls).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.Property(a => a.PostLogoutRedirectUris).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.Property(a => a.Contacts).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.Property(a => a.DefaultAcrValues).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
            builder.HasMany(c => c.OpenIdAllowedScopes).WithOne();
            builder.Ignore(c => c.AllowedScopes);
            builder.HasMany(c => c.Translations).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Scopes).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(c => c.ClientNames);
            builder.Ignore(c => c.LogoUris);
            builder.Ignore(c => c.ClientUris);
            builder.Ignore(c => c.PolicyUris);
            builder.Ignore(c => c.TosUris);
            builder.HasMany(c => c.JsonWebKeys).WithOne();
        }
    }
}