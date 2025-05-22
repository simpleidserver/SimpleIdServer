# Configuring Entity Framework Core for Different Databases

Entity Framework is an object-relational mapper (ORM) that simplifies database operations and management. 
By integrating EF with your SCIM project, you can ensure that identity and directory data is persisted in your chosen database system. Each supported database has its own NuGet package containing migration files that help set up the database schema. With a few configuration tweaks in your `Program.cs` file, you can quickly change your database backend to suit your deployment needs.

## SQL Server Configuration

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimSqlserver)

To configure SQL Server, install the NuGet package `SimpleIdServer.Scim.SqlServerMigrations`. 

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.SqlServerMigrations
```

Once installed, modify your `Program.cs` file to include the necessary setup. 
This involves calling the `UseEfStore` method and configuring the connection string along with EF’s SQL Server provider.

Here’s an example configuration for SQL Server:

```csharp  title="Program.cs"
var builder = WebApplication.CreateBuilder(args);
const string connectionString = "";
builder.Services.AddScim()
    .UseEfStore((db) =>
    {
        db.UseSqlServer(connectionString, o =>
        {
            o.MigrationsAssembly("SimpleIdServer.Scim.SqlServerMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });
var app = builder.Build();
app.UseScim();
app.Run();
```

This setup instructs Entity Framework to use SQL Server as the database engine, points to the migrations assembly, and sets the query splitting behavior to help optimize performance.

## PostgreSQL Configuration

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimPostgresql)

For those wishing to use PostgreSQL, the process is very similar. Install the NuGet package `SimpleIdServer.Scim.PostgreMigrations` and configure the project through the `UseEfStore` method.

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.PostgreMigrations
```

Adjust your connection string for PostgreSQL accordingly.

```csharp  title="Program.cs"
var builder = WebApplication.CreateBuilder(args);
const string connectionstring = "";
builder.Services.AddScim()
    .UseEfStore(o =>
    {
        o.UseNpgsql(connectionstring, o =>
        {
            o.MigrationsAssembly("SimpleIdServer.Scim.PostgreMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });
var app = builder.Build();
app.UseScim();
app.Run();
```

This example demonstrates the ease of switching providers; the key differences lie only in the specific NuGet package and the database connection provider method.

## MySQL Configuration

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimMysql)

To use MySQL, install the NuGet package `SimpleIdServer.Scim.MySQLMigrations`. 

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.MySQLMigrations
```

In your `Program.cs`, configure the store by calling `UseEfStore` with the MySQL provider. Note how the MySQL configuration includes additional options such as auto-detecting the server version and setting the schema behavior.

```csharp  title="Program.cs"
const string connectionString = "";
builder.Services.AddScim().UseEfStore(e =>
{
    e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
    {
        o.MigrationsAssembly("SimpleIdServer.Scim.MySQLMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        o.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
    });
});
var app = builder.Build();
app.UseScim();
app.Run();
```

This configuration allows Entity Framework to handle MySQL specifics—ensuring that your schema and query behavior are correctly managed for MySQL’s conventions.

## SQLite Configuration

[Source code](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimSqlite)

SQLite represents a slightly different case.
After installing the NuGet package `SimpleIdServer.Scim.SqliteMigrations`, you can configure SQLite much like the other providers.

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.SqliteMigrations
```

However, there is one important difference: SQLite does not support bulk operations. As such, you must disable these operations by setting the `IgnoreBulkOperation` property to true.
Below is an example configuration for SQLite:

```csharp  title="Program.cs"
const string connectionstring = "";
builder.Services.AddScim().UseEfStore(e =>
{
    e.UseSqlite(connectionstring, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.Scim.SqliteMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, c =>
{
    c.IgnoreBulkOperation = true;
});
var app = builder.Build();
app.UseScim();
app.Run();
```

This setup shows how to accommodate SQLite’s limitations while still benefitting from Entity Framework’s powerful migration and connection capabilities.