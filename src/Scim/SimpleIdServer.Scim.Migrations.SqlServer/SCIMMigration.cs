using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.Scim.Persistence.EF;

namespace SimpleIdServer.Scim.Migrations.SqlServer
{
    public class SCIMMigration : IDesignTimeDbContextFactory<SCIMDbContext>
    {
        public SCIMDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SCIMDbContext>();
            builder.UseSqlServer("Data Source=.;Initial Catalog=SCIM;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly("SimpleIdServer.Scim.Migrations.SqlServer"));
            return new SCIMDbContext(builder.Options);
        }
    }
}
