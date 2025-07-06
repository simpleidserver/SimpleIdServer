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

## Running the EF Migration

Once your SCIM server is configured and ready to use a database, you can trigger the EF Core migration by calling the `EnsureEfStoreMigrated` function in your `Program.cs` file.

```csharp title="Program.cs"
var app = builder.Build();
app.Services.EnsureEfStoreMigrated(new List<SCIMSchema>(), new List<SCIMAttributeMapping>(), new List<Realm>());;
```

This function takes three input parameters:

1. **List of SCIM Schemas** : You can use the `SCIMSchemaExtractor` class to extract SCIM schemas from a JSON file. For example:

```
var userSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
```

2. **List of attribute mappings** : These mappings define parent-child relationships between two types of representations—for example, between a `Group` and a `User`.

The code snippet below sets up the relationship from a `Group` to a `User`, where the `members` property of a `group` contains a list of `users`:

```csharp
new SCIMAttributeMapping
{
    Id = Guid.NewGuid().ToString(),
    SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
    SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
    SourceAttributeSelector = "members",
    TargetResourceType = StandardSchemas.UserSchema.ResourceType,
    TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
    Mode = Mode.PROPAGATE_INHERITANCE
}
```

The following code snippet sets up the inverse relationship from a `User` to a `Group`, where the `groups` property of a `user` contains a list of `groups` :

```
new SCIMAttributeMapping
{
    Id = Guid.NewGuid().ToString(),
    SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
    SourceResourceType = StandardSchemas.UserSchema.ResourceType,
    SourceAttributeSelector = "groups",
    TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
    TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id
}
```

3. **List of realms** : The third parameter defines the list of realms to be created. For more information, refer to the dedicated section on realms.