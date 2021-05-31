// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.EF;
using SimpleIdServer.Uma.Domains;

namespace SimpleIdServer.Uma.EF
{
    public class UMAEFDbContext : BaseOAuthDBContext<UMAEFDbContext>
    {
        public UMAEFDbContext(DbContextOptions<UMAEFDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<UMAPendingRequest> PendingRequests { get; set; }
        public DbSet<UMAResource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseClient>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OAuthDBContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
