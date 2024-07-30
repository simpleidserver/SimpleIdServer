// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.EF.Configurations;

public class RealmRoleScopeConfiguration : IEntityTypeConfiguration<RealmRoleScope>
{
    public void Configure(EntityTypeBuilder<RealmRoleScope> builder)
    {
        builder.HasKey(r => new { r.ScopeId, r.RealmRoleId });
    }
}
