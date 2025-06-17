// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF.Extensions;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(a => a.ClientRegistrationTypesSupported).ConfigureSerializer();
            builder.Property(a => a.GrantTypes).ConfigureSerializer();
            builder.Property(a => a.ResponseTypes).ConfigureSerializer();
            builder.Property(a => a.RedirectionUrls).ConfigureSerializer();
            builder.Property(a => a.PostLogoutRedirectUris).ConfigureSerializer();
            builder.Property(a => a.Contacts).ConfigureSerializer();
            builder.Property(a => a.DefaultAcrValues).ConfigureSerializer();
            builder.Property(a => a.AuthorizationDataTypes).ConfigureSerializer();
            builder.Property(a => a.SubjectSyntaxTypesSupported).ConfigureSerializer();
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
