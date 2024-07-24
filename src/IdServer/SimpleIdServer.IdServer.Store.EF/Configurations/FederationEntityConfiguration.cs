// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class FederationEntityConfiguration : IEntityTypeConfiguration<FederationEntity>
{
    public void Configure(EntityTypeBuilder<FederationEntity> builder)
    {
        builder.HasKey(e => e.Id);
    }
}