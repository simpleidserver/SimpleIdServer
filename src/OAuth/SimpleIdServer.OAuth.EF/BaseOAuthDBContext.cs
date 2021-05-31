// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.EF
{
    public class BaseOAuthDBContext<TContext> : DbContext where TContext : DbContext
    {
        public BaseOAuthDBContext(DbContextOptions<TContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<JsonWebKey> JsonWebKeys { get; set; }
        public DbSet<OAuthClient> OAuthClients { get; set; }
        public DbSet<OAuthUser> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<OAuthScope> OAuthScopes { get; set; }
    }
}
