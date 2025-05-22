# Configuring Entity Framework Core for Different Databases

Entity Framework Core is used here for data storage. For each type of database, there is a corresponding NuGet package that contains migration files used to create tables in the target database.

## SQL Server

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverSqlServer)

To configure SQL Server, install the NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.SqlServerMigrations
```

Then, in the `Program.cs` file, add the following lines:

```csharp  title="Program.cs"
const string connectionString = "";
...
.AddDeveloperSigningCredential()
.UseEfStore(e =>
{
    e.UseSqlServer(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, e =>
{
    e.UseSqlServer(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

## PostgreSQL

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverPostgre)

To configure PostgreSQL, install the NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.PostgreMigrations
```

Then, in the `Program.cs` file, add the following lines:

```csharp  title="Program.cs"
const string connectionString = "";
...
.AddDeveloperSigningCredential()
.UseEfStore(e =>
{
    e.UseNpgsql(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.PostgreMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, e =>
{
    e.UseNpgsql(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.PostgreMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

## MySQL

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverMysql)

To configure MySQL, install the NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.MySQLMigrations
```

Then, in the `Program.cs` file, add the following lines:

```csharp  title="Program.cs"
const string connectionString = "";
...
.AddDeveloperSigningCredential()
.UseEfStore(e =>
{
    e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, e =>
{
    e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

Additionally, be sure to add a `DbContextFactory` class, as the migration will not work without it.

```csharp  title="DbContextFactory.cs"
public class DbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        var connectionString = "server=localhost;port=3306;database=idserver;user=admin;password=tJWBx3ccNJ6dyp1wxoA99qqQ";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
        {
            o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
        return new StoreDbContext(optionsBuilder.Options);
    }
}
```

## SQLite

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverSqlite)

To configure SQLite, install the NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.SqliteMigrations
```

Then, in the `Program.cs` file, add the following lines:

```csharp  title="Program.cs"
const string connectionString = "";
.AddDeveloperSigningCredential()
.UseEfStore(e =>
{
    e.UseSqlite(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqliteMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, e =>
{
    e.UseSqlite(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqliteMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

With these configurations, the necessary migration files will be used to create and manage the database tables for your chosen database system using Entity Framework Core.