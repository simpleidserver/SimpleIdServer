// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF.Extensions;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
    {
        public void Configure(EntityTypeBuilder<Consent> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Claims).ConfigureSerializer();
            builder.Ignore(c => c.AuthorizationDetails);
            builder.HasMany(g => g.Scopes).WithOne(g => g.Consent).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
