// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class GroupRealmRoleConfiguration : IEntityTypeConfiguration<GroupRealmRole>
{
    public void Configure(EntityTypeBuilder<GroupRealmRole> builder)
    {
        builder.HasKey(g => new { g.RealmRoleId, g.GroupId });
    }
}
