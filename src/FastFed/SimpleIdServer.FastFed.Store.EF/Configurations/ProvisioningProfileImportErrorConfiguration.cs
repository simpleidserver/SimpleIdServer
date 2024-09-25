// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.FastFed.Models;
using System;

namespace SimpleIdServer.FastFed.Store.EF.Configurations
{
    public class ProvisioningProfileImportErrorConfiguration : IEntityTypeConfiguration<ProvisioningProfileImportError>
    {
        public void Configure(EntityTypeBuilder<ProvisioningProfileImportError> builder)
        {
            builder.HasKey(b => b.Id);
        }
    }
}
