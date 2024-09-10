// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.ApplicationProvider.Models;
using SimpleIdServer.FastFed.ApplicationProvider.Store.EF.Configurations;

namespace SimpleIdServer.FastFed.ApplicationProvider.Store.EF;

public class FastFedDbContext : DbContext
{
    public FastFedDbContext(DbContextOptions<FastFedDbContext> options) : base(options) { }

    public DbSet<IdentityProviderFederation> IdentityProviderFederations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new IdentityProviderFederationConfiguration());
    }
}
