// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.Store
{
    public class OpenIdDbContext : DbContext
    {
        public OpenIdDbContext(DbContextOptions<OpenIdDbContext> options) : base(options) { }

        public DbSet<AuthenticationSchemeProvider> AuthenticationSchemeProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AuthenticationSchemeProviderConfiguration());
        }
    }
}
