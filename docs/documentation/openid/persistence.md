# Persistence

By default, all the assets like `Clients`, `Scopes`, `Users` and `JSON Web Keys` are stored in memory. The following data storage can be used. 

## SQLServer

> [!WARNING]
> A SimpleIdServer template exists to create OPENID server with EF support.
> * Execute the command line `dotnet new openidefbs4 -n OpenId`.
> * Create the migration scripts `dotnet ef migrations add Init`.
> * Update the `CONNECTIONSTRING` parameter in the files `OpenIDMigration.cs` and `Startup.cs` before running the solution.

**Pre-requisite** : [OPENID server must be installed](/documentation/openid/installation.html) in the Visual Studio Solution.

SQL Server data storage can be configured like this : 

* In a command prompt, navigate to the directory `src\OpenId`.
* Install the Nuget package `SimpleIdServer.OpenID.EF`.

```
dotnet add package SimpleIdServer.OpenID.EF
```

* Install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
* Install the Nuget package `Microsoft.EntityFrameworkCore.Design`.

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

* Create an `OpenIdMigration` class and replace the content with the following code. The CONNECTIONSTRING must be updated :

```
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
```

* Execute the following command line, the migration scripts will be created

```
dotnet ef migrations add Init
```

* Execute the following command line, the tables will be created in the database :

```
dotnet ef database update
```

* Open the `Startup.cs` file, replace any existing calls to `AddClients`, `AddAcrs`, `AddUsers`, `AddJsonWebKeys` by `AddOpenIDEF`. The `CONNECTIONSTRING` must be updated.

```
services
    .AddSIDOpenID()
    .AddOpenIDEF(opt => opt.UseSqlServer("<<CONNECTIONSTRING>>", o => o.MigrationsAssembly(typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name)))
    .AddLoginPasswordAuthentication();
```