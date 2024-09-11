// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.FastFed.Models;

namespace SimpleIdServer.FastFed.Store.EF.Configurations;

public class IdentityProviderFederationCapabilitiesConfiguration : IEntityTypeConfiguration<IdentityProviderFederationCapabilities>
{
    public void Configure(EntityTypeBuilder<IdentityProviderFederationCapabilities> builder)
    {
        builder.HasKey(i => i.Id);
    }
}
