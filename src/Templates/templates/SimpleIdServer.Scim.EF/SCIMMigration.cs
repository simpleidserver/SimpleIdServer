using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.Scim.Persistence.EF;
using System.Reflection;

namespace SimpleIdServer.Scim.EF
{
    public class SCIMMigration : IDesignTimeDbContextFactory<SCIMDbContext>
    {
        public SCIMDbContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<SCIMDbContext>();
            builder.UseSqlServer("<<CONNECTIONSTRING>>", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new SCIMDbContext(builder.Options);
        }
    }
}
