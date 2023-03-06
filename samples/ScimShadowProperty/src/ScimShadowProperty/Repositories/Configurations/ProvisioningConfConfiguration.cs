// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;

namespace ScimShadowProperty.Repositories.Configurations
{
    public class ProvisioningConfConfiguration : IEntityTypeConfiguration<ProvisioningConfiguration>
    {
        public void Configure(EntityTypeBuilder<ProvisioningConfiguration> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasMany(p => p.Records).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.HistoryLst).WithOne(v => v.ProvisioningConfiguration).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
