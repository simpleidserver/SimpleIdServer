// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Idp.Domains;

namespace SimpleIdServer.Saml.Idp.EF
{
    public class SamlIdpDBContext : DbContext
    {
        public SamlIdpDBContext(DbContextOptions<SamlIdpDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<RelyingPartyAggregate> RelyingParties { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
