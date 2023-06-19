// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Ethr.Store.Configurations;

namespace SimpleIdServer.Did.Ethr.Store
{
    public class EthrDbContext : DbContext
    {
        public EthrDbContext(DbContextOptions<EthrDbContext> options) : base(options) { }

        public DbSet<NetworkConfiguration> Networks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new NetworkConfigurationConf());
        }
    }
}
