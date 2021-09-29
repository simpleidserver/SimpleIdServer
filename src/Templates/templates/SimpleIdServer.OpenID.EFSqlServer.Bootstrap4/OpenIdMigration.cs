using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.OpenID.EF;
using System.Reflection;

namespace SimpleIdServer.OpenID.EFSqlServer.Bootstrap4
{
    public class OpenIDMigration : IDesignTimeDbContextFactory<OpenIdDBContext>
    {
        public OpenIdDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<OpenIdDBContext>();
            builder.UseSqlServer("<<CONNECTIONSTRING>>", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new OpenIdDBContext(builder.Options);
        }
    }
}
