// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class CertificateAuthorityConfiguration : IEntityTypeConfiguration<CertificateAuthority>
    {
        public void Configure(EntityTypeBuilder<CertificateAuthority> builder)
        {
            builder.HasKey(b => b.Id);
            builder.HasMany(b=> b.ClientCertificates).WithOne(b => b.CertificateAuthority).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(b => b.Realms).WithMany(b => b.CertificateAuthorities);
        }
    }
}
