// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Webfinger.Models;

namespace SimpleIdServer.Webfinger.Store.EF.Configurations;

public class WebfingerResourceConfiguration : IEntityTypeConfiguration<WebfingerResource>
{
    public void Configure(EntityTypeBuilder<WebfingerResource> builder)
    {
        builder.HasKey(x => x.Id);
    }
}