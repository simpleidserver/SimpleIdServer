// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Store.EF.Configurations;

namespace SimpleIdServer.FastFed.Store.EF;

public class FastFedDbContext : DbContext
{
    public FastFedDbContext(DbContextOptions<FastFedDbContext> options) : base(options) { }

    public DbSet<IdentityProviderFederation> IdentityProviderFederations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new IdentityProviderFederationConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityProviderFederationCapabilitiesConfiguration());
        modelBuilder.ApplyConfiguration(new CapabilitySettingsConfiguration());
    }
}
