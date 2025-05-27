using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Startup.Config;

public class FormBuilderSetup
{
    public static void ConfigureFormBuilder(WebApplicationBuilder builder, string cookieName)
    {
        var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        builder.Services.AddFormBuilder().UseEf((db) =>
        {
            ConfigureStorage(conf, db);
        }).AddTailwindcss();
        builder.Services.Configure<FormBuilderStartupOptions>(cb => cb.AntiforgeryCookieName = cookieName);
    }

    private static void ConfigureStorage(StorageConfiguration configuration, DbContextOptionsBuilder builder)
    {
        switch(configuration.Type)
        {
            case StorageTypes.INMEMORY:
                builder.UseInMemoryDatabase("formBuidler");
                break;
            case StorageTypes.SQLSERVER:
                builder.UseSqlServer(configuration.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SidFormBuilder.SqlServerMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.SQLITE:
                builder.UseSqlite(configuration.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SidFormBuilder.SqliteMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.POSTGRE:
                builder.UseNpgsql(configuration.ConnectionString, o =>
                {
                    o.MigrationsAssembly("SidFormBuilder.PostgreMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
            case StorageTypes.MYSQL:
                builder.UseMySql(configuration.ConnectionString, ServerVersion.AutoDetect(configuration.ConnectionString), o =>
                {
                    o.MigrationsAssembly("SidFormBuilder.MySQLMigrations");
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
        }
    }
}
