// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Store.EF.Configurations;

namespace SimpleIdServer.OpenidFederation.Store.EF;

public class OpenidFederationDbContext : DbContext
{
    public OpenidFederationDbContext(DbContextOptions<OpenidFederationDbContext> options) : base(options) { }

    public DbSet<FederationEntity> FederationEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new FederationEntityConfiguration());
    }
}