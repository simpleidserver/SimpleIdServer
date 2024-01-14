// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class RealmConfiguration : IEntityTypeConfiguration<Realm>
    {
        public void Configure(EntityTypeBuilder<Realm> builder)
        {
            builder.HasKey(r => r.Name);
            builder.HasMany(u => u.Users).WithOne(u => u.Realm).HasForeignKey(u => u.RealmsName);
            builder.HasMany(u => u.Groups).WithOne(u => u.Realm).HasForeignKey(g => g.RealmsName);
            builder.HasMany(u => u.CredentialTemplates).WithMany(u => u.Realms);
            builder.HasMany(u => u.RegistrationWorkflows).WithOne(u => u.Realm).HasForeignKey(u => u.RealmName);
        }
    }
}
