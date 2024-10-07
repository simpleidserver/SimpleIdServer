using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Store.EF;

/*
namespace SimpleIdServer.IdServer.PostgreMigrations;
public class DbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        var connectionString = "server=localhost;port=3306;database=idserver;user=root;password=password";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
        {
            o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
        return new StoreDbContext(optionsBuilder.Options);
    }
}
*/