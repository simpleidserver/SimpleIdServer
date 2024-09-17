// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.FastFed.Models;

namespace SimpleIdServer.FastFed.Store.EF.Configurations;

public class ProvisioningProfileHistoryConfiguration : IEntityTypeConfiguration<ProvisioningProfileHistory>
{
    public void Configure(EntityTypeBuilder<ProvisioningProfileHistory> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
