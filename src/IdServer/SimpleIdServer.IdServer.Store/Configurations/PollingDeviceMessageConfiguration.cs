// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class PollingDeviceMessageConfiguration : IEntityTypeConfiguration<PollingDeviceMessage>
    {
        public void Configure(EntityTypeBuilder<PollingDeviceMessage> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(a => a.Scopes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Ignore(a => a.Permissions);
        }
    }
}
