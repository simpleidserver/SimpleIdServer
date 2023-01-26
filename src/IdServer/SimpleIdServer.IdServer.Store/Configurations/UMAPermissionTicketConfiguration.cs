// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UMAPermissionTicketConfiguration : IEntityTypeConfiguration<UMAPermissionTicket>
    {
        public void Configure(EntityTypeBuilder<UMAPermissionTicket> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasMany(p => p.Records).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
