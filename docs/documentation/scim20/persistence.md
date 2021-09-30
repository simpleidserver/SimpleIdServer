# Persistence

By default, all the assets like `Representation`, `Schemas` are stored in memory. The following data storage can be used. 

## SQLServer

> [!WARNING]
> A SimpleIdServer template exists to create SCIM server with EF support.
> * Execute the command line `dotnet new scimef -n ScimHost`.
> * Create the migration scripts `dotnet ef migrations add Init`.
> * Update the `CONNECTIONSTRING` parameter in the files `ScimMigration.cs` and `Startup.cs` before running the solution.


**Pre-requisite** : [SCIM server must be installed](/documentation/scim20/installation.html) in the Visual Studio Solution.

SQL Server data storage can be configured like this : 

* In a command prompt, navigate to the directory `src\ScimHost`.
* Install the Nuget package `SimpleIdServer.Scim.Persistence.EF`. 

```
dotnet add package SimpleIdServer.Scim.Persistence.EF
```

* Install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
* Install the Nuget package `Microsoft.EntityFrameworkCore.Design`.

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

* Create a `SCIMMigration` class and replace the content with the following code. The `CONNECTIONSTRING` must be updated :

```
public class SCIMMigration : IDesignTimeDbContextFactory<SCIMDbContext> 
{ 
    public SCIMDbContext CreateDbContext(string[] args) 
    { 
        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; 
        var builder = new DbContextOptionsBuilder<SCIMDbContext>(); 
        builder.UseSqlServer("<<CONNECTIONSTRING>>", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
        return new SCIMDbContext(builder.Options); 
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

*  Edit the `Startup.cs` file, remove any existing calls to `AddSchemas`, `AddAttributeMapping` and call `services.AddOpenIDEF()`. The `CONNECTIONSTRING` must be updated.

```
services.AddScimStoreEF(options =>
{
    options.UseSqlServer("<<CONNECTIONSTRING>>", o => o.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
});
```

* Create a new private `InitializeDatabase` method. It will be called by the `Configure` procedure in order to create the database add feed the data. Each SCIM Schema must be added into the SCIMSchemaLst collection. 

```
private void InitializeDatabase(IApplicationBuilder app)
{
    using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var context = scope.ServiceProvider.GetService<SCIMDbContext>())
        {
            context.Database.Migrate();
            var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Schemas");
            var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMConstants.SCIMEndpoints.User, true);
            if (!context.SCIMSchemaLst.Any())
            {
                context.SCIMSchemaLst.Add(userSchema);
            }

            context.SaveChanges();
        }
    }
}
```

* Run the SCIM server and verify JSON is returned when you browse the following url : [http://localhost:60002/Schemas](http://localhost:60002/Schemas).

```
cd src/ScimHost
dotnet run --urls=http://localhost:60002
```

## MongoDB

> [!WARNING]
> A SimpleIdServer template exists to create SCIM server with MongoDB support. 
> * Execute the command line `dotnet new scimongodb -n ScimHost`.
> * Update  the `CONNECTIONSTRING` and `DATABASENAME` parameters in the `Startup.cs` file before running the solution.

**Pre-requisite** : [SCIM server must be installed](/documentation/scim20/installation.html) in the Visual Studio Solution.

MongoDB data storage can be configured like this : 

* In a command prompt, navigate to the directory `src\ScimHost`.
* Install the Nuget package `SimpleIdServer.Scim.Persistence.MongoDB`. 

```
dotnet add package SimpleIdServer.Scim.Persistence.MongoDB
```

* Edit the `Startup.cs` file, remove any existing calls to `AddSchemas`, `AddAttributeMapping` and call `services.AddScimStoreMongoDB()`. The `CONNECTIONSTRING` and `DATABASENAME` parameters must be updated.

```
services.AddScimStoreMongoDB(opt => 
{ 
	opt.ConnectionString = "<<CONNECTIONSTRING>>"; 
	opt.Database = "<<DATABASENAME>>";  
	opt.CollectionSchemas = “mappings”; 
	opt.CollectionSchemas = “schemas”; 
    opt.CollectionRepresentations = ”representations”; 
	opt.SupportTransaction = false; 
}, schemas); 
```

* Run the SCIM server and verify JSON is returned when you browse the following url : [http://localhost:60002/Schemas](http://localhost:60002/Schemas).

```
cd src/ScimHost
dotnet run --urls=http://localhost:60002
```