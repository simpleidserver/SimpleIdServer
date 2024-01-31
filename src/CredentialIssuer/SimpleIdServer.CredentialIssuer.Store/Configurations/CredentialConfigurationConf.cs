// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleIdServer.CredentialIssuer.Store.Configurations
{
    public class CredentialConfigurationConf : IEntityTypeConfiguration<Domains.CredentialConfiguration>
    {
        public void Configure(EntityTypeBuilder<Domains.CredentialConfiguration> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasMany(c => c.Claims).WithOne(c => c.CredentialConfiguration).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Displays).WithOne(c => c.CredentialConfiguration).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Credentials).WithOne(c => c.Configuration).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
