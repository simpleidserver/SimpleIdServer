/*
using FormBuilder.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace FormBuilder.Startup
{
    public class DbContextFactory : IDesignTimeDbContextFactory<FormBuilderDbContext>
    {
        public FormBuilderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FormBuilderDbContext>();
            var connectionString = "server=localhost;port=3306;database=idserver;user=admin;password=tJWBx3ccNJ6dyp1wxoA99qqQ";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
            {
                o.MigrationsAssembly("FormBuilder.MySQLMigrations");
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            return new FormBuilderDbContext(optionsBuilder.Options);
        }
    }
}
*/