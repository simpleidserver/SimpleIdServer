﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class MessageBusErrorMessageConfiguration : IEntityTypeConfiguration<MessageBusErrorMessage>
{
    public void Configure(EntityTypeBuilder<MessageBusErrorMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(a => a.Exceptions).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
    }
}