// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.EF
{
    public class OAuthDBContext : BaseOAuthDBContext<OAuthDBContext>
    {
        public OAuthDBContext(DbContextOptions<OAuthDBContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseClient>();
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
