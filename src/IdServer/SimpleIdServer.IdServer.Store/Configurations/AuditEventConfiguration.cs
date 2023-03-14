// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
    {
        public void Configure(EntityTypeBuilder<AuditEvent> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Scopes).HasConversion(v => string.Join(',', v), v => v.Split(",", StringSplitOptions.RemoveEmptyEntries));
            builder.Property(a => a.Claims).HasConversion(v => string.Join(',', v), v => v.Split(",", StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
