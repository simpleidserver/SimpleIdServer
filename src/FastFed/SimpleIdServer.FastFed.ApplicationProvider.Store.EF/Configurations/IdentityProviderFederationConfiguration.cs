// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.FastFed.ApplicationProvider.Models;

namespace SimpleIdServer.FastFed.ApplicationProvider.Store.EF.Configurations;

public class IdentityProviderFederationConfiguration : IEntityTypeConfiguration<IdentityProviderFederation>
{
    public void Configure(EntityTypeBuilder<IdentityProviderFederation> builder)
    {
        builder.HasKey(i => i.EntityId);
    }
}
