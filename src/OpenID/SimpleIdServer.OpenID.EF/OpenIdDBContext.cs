// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OpenID.EF
{
    public class OpenIdDBContext : BaseOpenIdDBContext<OpenIdDBContext>
    {
        public OpenIdDBContext(DbContextOptions<OpenIdDBContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseClient>();
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
