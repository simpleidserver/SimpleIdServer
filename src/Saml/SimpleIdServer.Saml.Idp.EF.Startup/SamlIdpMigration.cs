// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace SimpleIdServer.Saml.Idp.EF.Startup
{
    public class SamlIdpMigration : IDesignTimeDbContextFactory<SamlIdpDBContext>
    {
        public SamlIdpDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(SamlStartup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<SamlIdpDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=SamlIdp;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new SamlIdpDBContext(builder.Options);
        }
    }
}
