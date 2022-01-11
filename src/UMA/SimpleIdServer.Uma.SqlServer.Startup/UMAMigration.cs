// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.Uma.EF;
using System.Reflection;

namespace SimpleIdServer.Uma.SqlServer.Startup
{
    public class UMAMigration : IDesignTimeDbContextFactory<UMAEFDbContext>
    {
        public UMAEFDbContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(UmaStartup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<UMAEFDbContext>();
            builder.UseSqlServer("Data Source=DESKTOP-T4INEAM\\SQLEXPRESS;Initial Catalog=Uma;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new UMAEFDbContext(builder.Options);
        }
    }
}
