# Migrating from OpenIddict to SimpleIdServer

If you're currently using version 6 of the [OpenIddict](http://documentation.openiddict.com/) library and are considering switching to SimpleIdServer, migration is now possible thanks to a dedicated migration package.

## How to Migrate

To begin, install the NuGet package `SimpleIdServer.IdServer.Migrations.Openiddict`. This package provides everything needed to migrate your OpenIddict data to SimpleIdServer.

```cmd title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Migrations.Openiddict
```
Once the package is installed, update your `Program.cs` file to configure the migration services.

There are two essential methods to call:

* **EnableMigration()** – Registers the services required to perform a migration.

* **AddOpeniddictMigration(...)** – Registers the dependencies needed to migrate from OpenIddict to SimpleIdServer. This method takes a lambda expression to configure the database connection string used to access your existing OpenIddict database.

Here’s an example of how your Program.cs file might look with migration enabled:

```csharp title="Program.cs"
builder.AddSidIdentityServer()
    .EnableMigration()
    .AddOpeniddictMigration(a =>
    {
        a.UseSqlServer(""); // Add your connection string here
    });
```

## Running the migration

With the configuration in place, you're ready to run the migration and transfer your OpenIddict data into SimpleIdServer. 
Make sure to replace the placeholder connection string with the one that connects to your current OpenIddict database.

For additional guidance on how to execute the migration process, you can refer to the [Duende migration](./duende.md) documentation, which provides helpful context for understanding the overall flow.