// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.Scim.Persistence.EF;
using System.Reflection;

namespace SimpleIdServer.Scim.Postgre.Startup
{
    public class SCIMMigration : IDesignTimeDbContextFactory<SCIMDbContext>
    {
        public SCIMDbContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<SCIMDbContext>();
            builder.UseNpgsql("Host=localhost;Database=SCIM;Username=admin;Password=admin",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new SCIMDbContext(builder.Options);
        }
    }
}
