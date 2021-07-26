// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.OpenID.EF;
using System.Reflection;

namespace SimpleIdServer.OpenID.SqlServer.Startup
{
    public class OpenIDMigration : IDesignTimeDbContextFactory<OpenIdDBContext>
    {
        public OpenIdDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<OpenIdDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-T4INEAM\\SQLEXPRESS;Initial Catalog=OpenID;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new OpenIdDBContext(builder.Options);
        }
    }
}
