# Persistence

By default, all the assets like `Clients`, `Scopes`, `JSON Web Keys` and `UMA Resources` are stored in memory. The following data storage can be used.

## SQLServer

> [!WARNING]
> A SimpleIdServer template exists to create UMA2.0 server with EF support.
> * Execute the command line `dotnet new umaef -n UmaHost`.
> * Create the migration scripts `dotnet ef migrations add Init`.
> * Update the `CONNECTIONSTRING` parameter in the files `UmaMigration.cs` and `UmaStartup.cs` before running the solution.

**Pre-requisite** : [UMA server must be installed](/documentation/uma20/installation.html) in the Visual Studio Solution.

SQL Server data storage can be configured like this :

* In a command prompt, navigate to the directory `src\UmaHost`.
* Install the Nuget package `SimpleIdServer.Uma.EF`

```
dotnet add package SimpleIdServer.Uma.EF
```

* Install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

* Install the Nuget package `Microsoft.EntityFrameworkCore.Design`.

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

* Create a `UmaMigration` class  and replace the content with the following code. The `CONNECTIONSTRING` must be updated :

```
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
```

* Execute the following command line, the migration scripts will be created

```
dotnet ef migrations add Init
```

* Execute the following command line, the tables will be created in the database :

```
dotnet ef database update
```

* Open the `Startup.cs` file, replace any existing calls to `AddScopes`, `AddUmaResources`, `AddClients` and `AddJsonWebKeys` by `AddSIDUmaEF`. The `CONNECTIONSTRING` must be udated :

```
AddSIDUmaEF(opt =>
{
    opt.UseSqlServer("<<CONNECTIONSTRING>>", o => o.MigrationsAssembly(typeof(UmaStartup).GetTypeInfo().Assembly.GetName().Name));
})
```

* Create a new private `InitializeDatabase` method. It will be called by the `Configured` procedure in order to create the database and feed the data. 

```
private void InitializeDatabase(IApplicationBuilder app)
{
    using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var context = scope.ServiceProvider.GetService<UMAEFDbContext>())
        {
            context.Database.Migrate();
            if (context.Resources.Any())
            {
                return;
            }

            var oauthJsonWebKey = ExtractOAuthJsonWebKey();
            context.Resources.AddRange(UmaDefaultConfiguration.Resources);
            context.OAuthClients.AddRange(UmaDefaultConfiguration.DefaultClients);
            context.OAuthScopes.AddRange(UmaDefaultConfiguration.DefaultScopes);
            context.JsonWebKeys.Add(oauthJsonWebKey);
            context.SaveChanges();
        }
    }
}
```

* Run the UMA server and verify JSON is returned when you browse the following URL :

```
cd src/UmaHost
dotnet run --urls=http://localhost:60003
```
