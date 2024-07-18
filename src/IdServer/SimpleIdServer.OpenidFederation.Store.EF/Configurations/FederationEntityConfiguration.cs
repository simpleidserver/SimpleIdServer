// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.OpenidFederation.Store.EF.Configurations;

public class FederationEntityConfiguration : IEntityTypeConfiguration<FederationEntity>
{
    public void Configure(EntityTypeBuilder<FederationEntity> builder)
    {
        builder.HasKey(f => f.Id);
    }
}
