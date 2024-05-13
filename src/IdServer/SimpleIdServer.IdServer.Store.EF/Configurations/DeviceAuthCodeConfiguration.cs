// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class DeviceAuthCodeConfiguration : IEntityTypeConfiguration<DeviceAuthCode>
    {
        public void Configure(EntityTypeBuilder<DeviceAuthCode> builder)
        {
            builder.HasKey(d => d.DeviceCode);
            builder.Property(a => a.Scopes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
        }
    }
}
