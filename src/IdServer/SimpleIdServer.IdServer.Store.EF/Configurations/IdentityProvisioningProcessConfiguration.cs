// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class IdentityProvisioningProcessConfiguration : IEntityTypeConfiguration<IdentityProvisioningProcess>
    {
        public void Configure(EntityTypeBuilder<IdentityProvisioningProcess> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Ignore(p => p.IsExported);
            builder.Ignore(p => p.IsImported);
        }
    }
}
