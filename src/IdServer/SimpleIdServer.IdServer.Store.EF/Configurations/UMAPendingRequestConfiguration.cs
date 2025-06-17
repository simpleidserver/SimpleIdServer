// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF.Extensions;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UMAPendingRequestConfiguration : IEntityTypeConfiguration<UMAPendingRequest>
    {
        public void Configure(EntityTypeBuilder<UMAPendingRequest> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.Scopes).ConfigureSerializer();
            builder.HasOne(p => p.Resource).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
