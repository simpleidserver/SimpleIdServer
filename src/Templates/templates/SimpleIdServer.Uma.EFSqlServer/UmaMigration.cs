using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.Uma.EF;
using System.Reflection;

namespace SimpleIdServer.Uma.EFSqlServer
{
    public class UmaMigration : IDesignTimeDbContextFactory<UMAEFDbContext>
    {
        public UMAEFDbContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(UmaStartup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<UMAEFDbContext>();
            builder.UseSqlServer("<<CONNECTIONSTRING>>", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new UMAEFDbContext(builder.Options);
        }
    }
}
