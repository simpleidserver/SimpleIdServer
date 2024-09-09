// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Webfinger.Models;
using SimpleIdServer.Webfinger.Store.EF.Configurations;

namespace SimpleIdServer.Webfinger.Store.EF;

public class WebfingerDbContext : DbContext
{
    public WebfingerDbContext(DbContextOptions<WebfingerDbContext> options) : base(options) { }

    public DbSet<WebfingerResource> WebfingerResources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WebfingerResourceConfiguration());
    }
}
